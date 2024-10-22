using AutoMapper;
using Entity;
using MatchManagement.DTOs;
using MatchManagement.DTOs.MatchDto;
using MatchManagement.DTOs.MatchDto.ViewDto;
using MatchManagement.Repository;
using MatchManagement.Ultility;

namespace MatchManagement.Service.Impl;

public class MatchService(IUnitOfWork unitOfWork, IMapper mapper, Validate validate, ListExtensions listExtensions) : IMatchService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly Validate _validate = validate;
    private readonly ListExtensions _listExtensions = listExtensions;
    
    public async Task<ResponseDto> GetMatches(string? subject, int? subjectId, string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber,
        int pageSize)
    {
        var responseDto = new ResponseDto(null, "Get successfully", true, StatusCodes.Status200OK);
        try
        {
            List<MatchViewsDto>? matches;
            int total;
            
            if (_validate.IsEmptyOrWhiteSpace(filterField) || _validate.IsEmptyOrWhiteSpace(filterValue))
            {
                if (!_validate.IsEmptyOrWhiteSpace(subject) && subjectId != null)
                {
                    matches = await _unitOfWork.MatchRepo.FindByConditionWithPagingAsync( m => m.CreateBy == subjectId,
                        m => 
                        new MatchViewsDto(
                            m.MatchId, 
                            m.Sport.SportName, 
                            m.MatchName, 
                            m.CreateByNavigation.UserName,
                            m.MatchType,
                            m.TeamSize,
                            m.MinLevel,
                            m.MaxLevel,
                            m.Date,
                            m.TimeStart,
                            m.TimeEnd,
                            m.Location!,
                            m.Status ?? 0
                        ), pageNumber, pageSize);
                }
                else
                {
                    matches = await _unitOfWork.MatchRepo.FindAllAsync(m => 
                        new MatchViewsDto(
                            m.MatchId, 
                            m.Sport.SportName, 
                            m.MatchName, 
                            m.CreateByNavigation.UserName,
                            m.MatchType,
                            m.TeamSize,
                            m.MinLevel,
                            m.MaxLevel,
                            m.Date,
                            m.TimeStart,
                            m.TimeEnd,
                            m.Location!,
                            m.Status ?? 0
                        ), pageNumber, pageSize);
                }

                total = matches.Count;
            }
            else
            {
                matches = await _unitOfWork.MatchRepo.GetMatches(filterField!, filterValue!, pageNumber, pageSize);
                total = matches.Count;
            }
            
            matches = Sort(matches, sortField, sortValue);

            responseDto.Result = new { matches, total};
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    public async Task<ResponseDto> EnrollInMatch(int userId, int matchId)
    {
        var responseDto = new ResponseDto(null, "", true, StatusCodes.Status302Found);
        try
        {
            //check user in database
            var existedUser = await _unitOfWork.UserRepo.AnyAsync(u => u.UserId == userId);
            if (!existedUser)
            {
                responseDto.Message = "There are no users with this id";
                responseDto.IsSucceed = false;
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                return responseDto;
            }
            
            //check match in database
            var matchEnroll = await _unitOfWork.MatchRepo.GetByConditionAsync(m => m.MatchId == matchId,
                m => new MatchEnrollDto(
                m.CreateBy,
                m.SportId,
                m.TimeStart,
                m.TimeEnd,
                m.Date,
                m.Mode,
                m.Gender,
                m.MaxAge,
                m.MinAge,
                m.MinLevel,
                m.MaxLevel
            ));
            
            if (matchEnroll == null)
            {
                responseDto.Message = "There are no matches with this id";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                responseDto.IsSucceed = false;
                return responseDto;
            }

            switch (matchEnroll.Mode)
            {
                case 1:
                {
                    var check = _unitOfWork.FriendShipRepo.AnyAsync(fs => 
                            fs.User1Id == userId && 
                            fs.User2Id == matchEnroll.UserId ||
                            fs.User1Id == matchEnroll.UserId && 
                            fs.User2Id == userId)
                    
                        .Result;
                    if (!check)
                    {
                        responseDto.Message = "This match is only for friends of the match owner.";
                        responseDto.StatusCode = StatusCodes.Status400BadRequest;
                        responseDto.IsSucceed = false;
                        return responseDto;
                    }

                    break;
                }
                case 2:
                    
                    break;
            }

            //overall validation
            responseDto = await ValidateForEnrolling(userId, matchEnroll);

            if (responseDto.IsSucceed == false) return responseDto;

            //enroll user into match
            await _unitOfWork.UserMatchRepo.CreateAsync(new UserMatch(){UserId = userId, MatchId = matchId, Status = 0});

            responseDto.Message = "Enroll successfully";
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    public async Task<ResponseDto> UnEnrollFromMatch(int userId, int matchId)
    {
        var responseDto = new ResponseDto(null, "UnEnroll successfully", true, StatusCodes.Status302Found);
        try
        {
            //check user in database
            var existedUser = await _unitOfWork.UserRepo.AnyAsync(u => u.UserId == userId);

            if (!existedUser)
            {
                responseDto.Message = "There are no users with this id";
                responseDto.IsSucceed = false;
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                return responseDto;
            }
            
            //get match fields in database
            var match = await _unitOfWork.MatchRepo.GetByConditionAsync(m => m.MatchId == matchId,
            m => new
            {
                m.TimeStart,
                m.BlockingOff,
                m.Status
            });

            //check match is null or not
            if (match == null)
            {
                responseDto.Message = "There are no matches with this id";
                responseDto.IsSucceed = false;
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                return responseDto;
            }

            //check user is in match or not
            var userMatch =
                await _unitOfWork.UserMatchRepo.GetByConditionAsync(um => um.UserId == userId && um.MatchId == matchId, 
                    um => um);

            if (userMatch == null)
            {
                responseDto.Message = "This user does not enroll in the match yet.";
                responseDto.IsSucceed = false;
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                return responseDto;
            }

            //check if user is an owner of the match
            if (userMatch.Status == 0)
            {
                responseDto.Message = "User is owner of the match. Cannot un-enroll";
                responseDto.IsSucceed = false;
                responseDto.StatusCode = StatusCodes.Status400BadRequest;
                return responseDto;
            }

            //check match status is available or not
            if (match.Status!.Value != 0)
            {
                responseDto.Message = "Match must be available";
                responseDto.IsSucceed = false;
                responseDto.StatusCode = StatusCodes.Status400BadRequest;
                return responseDto;
            }
            

            if (match.BlockingOff.HasValue)
            {
                //get the minimum time to un-enroll from the match
                var minTimeToUnEnroll = match.TimeStart.AddHours(-match.BlockingOff.Value);

                var timeNow = TimeOnly.FromDateTime(DateTime.Now);

                //compare now to the min time for un-enroll from the match
                if (timeNow >= minTimeToUnEnroll)
                {
                    responseDto.Message = "UnEnrollment is disable because user is in blocking off period";
                    responseDto.IsSucceed = false;
                    responseDto.StatusCode = StatusCodes.Status400BadRequest;
                    return responseDto;
                }
            }

            await _unitOfWork.UserMatchRepo.UnEnrollment(userMatch);
            
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    public async Task<ResponseDto> GetMatchById(int id)
    {
        var responseDto = new ResponseDto(null, "Get successfully", true, StatusCodes.Status302Found);
        try
        {
            var match = await _unitOfWork.MatchRepo.GetByConditionAsync(m => m.MatchId == id,
                m => m, m => m.Sport);
            
            if (match == null)
            {
                responseDto.Message = "There are no matches with this id";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
            }

            responseDto.Result = _mapper.Map<MatchViewDto>(match);
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    public async Task<ResponseDto> CreateMatch(int userId, MatchCreateDto matchCreateDto)
    {
        var responseDto = new ResponseDto(null, "Create successfully", true, StatusCodes.Status201Created);
        try
        {
            //overall validation
            responseDto = await ValidateForCreating(userId, matchCreateDto);
            if (responseDto.IsSucceed == false)
            {
                return responseDto;
            }

            var match = _mapper.Map<Match>(matchCreateDto);

            match.CreateBy = userId;
            match.Status = 0;

            await _unitOfWork.MatchRepo.CreateAsync(match);

            await _unitOfWork.UserMatchRepo.CreateAsync(new UserMatch
            {
                MatchId = match.MatchId,
                UserId = userId,
                Status = 0
            });
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    public async Task<ResponseDto> UpdateMatch(int id, MatchUpdateDto matchUpdateDto)
    {
        var responseDto = new ResponseDto(null, "Update successfully", true, StatusCodes.Status200OK);
        try
        {
            //check match in database
            var match = await _unitOfWork.MatchRepo.GetByConditionAsync(m => m.MatchId == id, m => m);
            if (match == null)
            {
                responseDto.Message = "There are no matches with this id";
                responseDto.IsSucceed = false;
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                return responseDto;
            }
            
            //overall validation
            responseDto = await ValidateForUpdating(id, matchUpdateDto);
            if (responseDto.IsSucceed == false)
            {
                return responseDto;
            }

            match = _mapper.Map(matchUpdateDto, match);

            await _unitOfWork.MatchRepo.UpdateAsync(match);
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    public async Task<ResponseDto> DeleteMatch(int id)
    {
        var responseDto = new ResponseDto(null, "Delete successfully", true, StatusCodes.Status200OK);
        try
        {
            var match = await _unitOfWork.MatchRepo.GetByConditionAsync(m => m.MatchId == id, m => m);
            if (match == null)
            {
                responseDto.Message = "There are no matches with this id";
                responseDto.IsSucceed = false;
                responseDto.StatusCode = StatusCodes.Status404NotFound;
            }
            else
            {
                await _unitOfWork.MatchRepo.DeleteAsync(match);
            }
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    private async Task<ResponseDto> ValidateForEnrolling(int userId, MatchEnrollDto matchEnrollDto)
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {
            
            var userEnroll = await _unitOfWork.UserRepo.GetByConditionAsync(u => u.UserId == userId,
                u => new
            {
                u.Gender,
                u.Dob
            });
            
            //check overlap matches
            var existedMatches = await _unitOfWork.UserMatchRepo.AnyAsync(
                um => um.UserId == userId &&
                      um.Match.Date == matchEnrollDto.Date &&
                      um.Match.TimeStart < matchEnrollDto.TimeEnd &&
                      um.Match.TimeEnd > matchEnrollDto.TimeStart);

            if (existedMatches)
            {
                response.Message = "There are matches which the user enrolled in overlaps with the new one";
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.IsSucceed = false;
                return response;
            }
            
            sbyte? level;

            //check min level
            if (matchEnrollDto.MinLevel.HasValue)
            {
                level = await _unitOfWork.UserSportRepo.GetByConditionAsync(us => us.SportId == matchEnrollDto.SportId &&
                                                                             us.UserId == userId, us => us.Level);

                if (level == null)
                {
                    response.Message = "User level is not suitable";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.IsSucceed = false;
                    return response;
                }
                
                if (matchEnrollDto.MinLevel >= level.Value )
                {
                    response.Message = "User level is not suitable";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.IsSucceed = false;
                    return response;
                }
            }
            
            //check max level
            if (matchEnrollDto.MaxLevel.HasValue)
            {
                level = await _unitOfWork.UserSportRepo.GetByConditionAsync(us => us.SportId == matchEnrollDto.SportId &&
                    us.UserId == userId, us => us.Level);

                if (level == null)
                {
                    response.Message = "User level is not suitable";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.IsSucceed = false;
                    return response;
                }
                
                if (matchEnrollDto.MaxLevel.Value < level.Value )
                {
                    response.Message = "User level is not suitable";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.IsSucceed = false;
                    return response;
                }
            }

            var age = (sbyte) (DateTime.Now.Year - userEnroll!.Dob.Year);
            
            //check min age
            if (matchEnrollDto.MinAge.HasValue)
            {
                if (matchEnrollDto.MinAge > age )
                {
                    response.Message = "User age is not suitable";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.IsSucceed = false;
                    return response;
                }
            }
            
            //check max age
            if (matchEnrollDto.MaxAge.HasValue)
            {
                if (matchEnrollDto.MaxAge < age )
                {
                    response.Message = "User age is not suitable";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.IsSucceed = false;
                    return response;
                }
            }
            
            //check gender
            if (matchEnrollDto.Gender != null && _validate.IsEmptyOrWhiteSpace(matchEnrollDto.Gender))
            {
                if (!userEnroll.Gender.Equals(matchEnrollDto.Gender, StringComparison.OrdinalIgnoreCase))
                {
                    response.Message = "User gender is not suitable";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.IsSucceed = false;
                    return response;
                }
            }
        }
        catch (Exception e)
        {
            response.Message = e.Message;
            response.IsSucceed = false;
            response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return response;
    }

    private async Task<ResponseDto> ValidateForCreating(int userId, MatchCreateDto matchCreateDto)
    {
        var responseDto = new ResponseDto(null, "Validate successfully", true, StatusCodes.Status201Created);
        try
        {
            
            //check sport in database
            var existedSport = await _unitOfWork.SportRepo.AnyAsync(s => s.SportId == matchCreateDto.SportId);
            if (!existedSport)
            {
                responseDto.Message = "There are no sports with this id";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                responseDto.IsSucceed = false;
                return responseDto;
            }
            
            //check overlap matches
            var checkOverlap = await IsOverlap(userId, matchCreateDto.Date, matchCreateDto.TimeStart,
                matchCreateDto.TimeEnd);
            
            if (checkOverlap)
            {
                responseDto.Message = "The new match time overlaps with an existing match.";
                responseDto.StatusCode = StatusCodes.Status400BadRequest;
                responseDto.IsSucceed = false;
                return responseDto;
            }
            

        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }
    
    private async Task<ResponseDto> ValidateForUpdating(int id, MatchUpdateDto matchUpdateDto)
    {
        var responseDto = new ResponseDto(null, "Validate successfully", true, StatusCodes.Status201Created);
        try
        {
            var existedSport = await _unitOfWork.SportRepo.AnyAsync(s => s.SportId == matchUpdateDto.SportId);
            if (!existedSport)
            {
                responseDto.Message = "There are no sports with this id";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                responseDto.IsSucceed = false;
                return responseDto;
            }

            //get owner of the match
            var userId = await _unitOfWork.MatchRepo.GetByConditionAsync(m => m.MatchId == id,
                m => m.CreateBy);

            //check overlap matches
            var checkOverlap = await IsOverlapForUpdate(id, userId, matchUpdateDto.Date, matchUpdateDto.TimeStart,
                matchUpdateDto.TimeEnd);
            
            if (checkOverlap)
            {
                responseDto.Message = "The new match time overlaps with an existing match.";
                responseDto.StatusCode = StatusCodes.Status400BadRequest;
                responseDto.IsSucceed = false;
                return responseDto;
            }
            
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }
    
    private async Task<bool> IsOverlap(int userId, DateOnly date, TimeOnly newTimeStart, TimeOnly newTimeEnd)
    {
        return await _unitOfWork.UserMatchRepo.AnyAsync(
            um => um.UserId == userId &&
                  um.Match.Date == date &&
                  um.Match.TimeStart < newTimeEnd &&
                  um.Match.TimeEnd > newTimeStart);
    }
    
    private async Task<bool> IsOverlapForUpdate(int id, int userId, DateOnly date, TimeOnly newTimeStart, TimeOnly newTimeEnd)
    {
        return await _unitOfWork.UserMatchRepo.AnyAsync(
            um => um.MatchId != id &&
                  um.UserId == userId &&
                  um.Match.Date == date &&
                  um.Match.TimeStart < newTimeEnd &&
                  um.Match.TimeEnd > newTimeStart);
    }

   private List<MatchViewsDto>? Sort(List<MatchViewsDto>? matches, string? sortField, string? sortValue)
{
    if (matches == null || matches.Count == 0 || string.IsNullOrEmpty(sortField) || 
        string.IsNullOrEmpty(sortValue) || string.IsNullOrWhiteSpace(sortField) || string.IsNullOrWhiteSpace(sortValue))
    {
        return matches;
    }

    matches = sortField.ToLower() switch
    {
        "date" => sortValue.Equals("asc")
            ? _listExtensions.Sort(matches, m => m.Date, true)
            : _listExtensions.Sort(matches, m => m.Date, false),
        "timestart" => sortValue.Equals("asc")
            ? _listExtensions.Sort(matches, m => m.TimeStart, true)
            : _listExtensions.Sort(matches, m => m.TimeStart, true),
        "timeend" => sortValue.Equals("asc")
            ? _listExtensions.Sort(matches, m => m.TimeEnd, true)
            : _listExtensions.Sort(matches, m => m.TimeEnd, true),
        "status" => sortValue.Equals("asc")
            ? _listExtensions.Sort(matches, m => m.Status, true)
            : _listExtensions.Sort(matches, m => m.Status, true),
        "matchtype" => sortValue.Equals("asc")
            ? _listExtensions.Sort(matches, m => m.MatchType, true)
            : _listExtensions.Sort(matches, m => m.MatchType, true),
        "teamsize" => sortValue.Equals("asc")
            ? _listExtensions.Sort(matches, m => m.TeamSize, true)
            : _listExtensions.Sort(matches, m => m.TeamSize, true),
        "minlevel" => sortValue.Equals("asc")
            ? _listExtensions.Sort(matches, m => m.MinLevel, true)
            : _listExtensions.Sort(matches, m => m.MinLevel, true),
        "maxlevel" => sortValue.Equals("asc")
            ? _listExtensions.Sort(matches, m => m.MaxLevel, true)
            : _listExtensions.Sort(matches, m => m.MaxLevel, true),
        _ => matches 
    };

    return matches;
}

}