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
    public async Task<ResponseDto> GetMatches(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber,
        int pageSize)
    {
        var responseDto = new ResponseDto(null, "Get successfully", true, StatusCodes.Status200OK);
        try
        {
            List<MatchViewsDto>? matches;
            if (validate.IsEmptyOrWhiteSpace(filterField) || validate.IsEmptyOrWhiteSpace(filterValue))
            {
                matches = await unitOfWork.MatchRepo.FindAllAsync(m => 
                        new MatchViewsDto(
                            m.MatchId, 
                            m.Sport!.SportName, 
                            m.MatchName, 
                            m.MatchType,
                            m.TeamSize,
                            m.MinLevel,
                            m.MaxLevel,
                            m.Date,
                            m.TimeStart,
                            m.TimeEnd,
                            m.Location!,
                            m.Status ?? 0
                        ),
                    m => m.Sport!);
            }
            else
            {
                matches = await unitOfWork.MatchRepo.GetMatches(filterField, filterValue);
            }
            
            matches = Sort(matches, sortField, sortValue);

            matches = listExtensions.Paging(matches, pageNumber, pageSize);

            responseDto.Result = matches;
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
            var match = await unitOfWork.MatchRepo.GetByIdAsync(id, m => m, m => m.Sport!);
            if (match == null)
            {
                responseDto.Message = "There are no matches with this id";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
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

    public async Task<ResponseDto> CreateMatch(MatchCreateDto matchCreateDto)
    {
        var responseDto = new ResponseDto(null, "Create successfully", true, StatusCodes.Status201Created);
        try
        {
            //overall validation
            responseDto = await ValidateForCreating(matchCreateDto);
            if (responseDto.IsSucceed == false)
            {
                return responseDto;
            }

            var match = mapper.Map<Match>(matchCreateDto);

            await unitOfWork.MatchRepo.CreateAsync(match);
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
            //check match
            var match = await unitOfWork.MatchRepo.GetByIdAsync(id, m => m);
            if (match == null)
            {
                responseDto.Message = "There are no matches with this id";
                responseDto.IsSucceed = false;
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                return responseDto;
            }
            
            //overall validation
            responseDto = await ValidateForUpdating(matchUpdateDto);
            if (responseDto.IsSucceed == false)
            {
                return responseDto;
            }

            match = mapper.Map<Match>(matchUpdateDto);

            await unitOfWork.MatchRepo.CreateAsync(match);
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
            var match = await unitOfWork.MatchRepo.GetByIdAsync(id, m => m);
            if (match == null)
            {
                responseDto.Message = "There are no matches with this id";
                responseDto.IsSucceed = false;
                responseDto.StatusCode = StatusCodes.Status404NotFound;
            }
            else
            {
                await unitOfWork.MatchRepo.DeleteAsync(match);
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

    private async Task<ResponseDto> ValidateForCreating(MatchCreateDto matchCreateDto)
    {
        var responseDto = new ResponseDto(null, "Validate successfully", true, StatusCodes.Status201Created);
        try
        {
            var sport = await unitOfWork.SportRepo.GetByIdAsync(matchCreateDto.SportId, s => s.SportId);
            if (sport <= 0)
            {
                responseDto.Message = "There are no sports with this id";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
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
    
    private async Task<ResponseDto> ValidateForUpdating(MatchUpdateDto matchUpdateDto)
    {
        var responseDto = new ResponseDto(null, "Validate successfully", true, StatusCodes.Status201Created);
        try
        {
            var sport = await unitOfWork.SportRepo.GetByIdAsync(matchUpdateDto.SportId, s => s.SportId);
            if (sport <= 0)
            {
                responseDto.Message = "There are no sports with this id";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
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
            ? listExtensions.Sort(matches, m => m.Date, true)
            : listExtensions.Sort(matches, m => m.Date, false),
        "timestart" => sortValue.Equals("asc")
            ? listExtensions.Sort(matches, m => m.TimeStart, true)
            : listExtensions.Sort(matches, m => m.TimeStart, true),
        "timeend" => sortValue.Equals("asc")
            ? listExtensions.Sort(matches, m => m.TimeEnd, true)
            : listExtensions.Sort(matches, m => m.TimeEnd, true),
        "status" => sortValue.Equals("asc")
            ? listExtensions.Sort(matches, m => m.Status, true)
            : listExtensions.Sort(matches, m => m.Status, true),
        "matchtype" => sortValue.Equals("asc")
            ? listExtensions.Sort(matches, m => m.MatchType, true)
            : listExtensions.Sort(matches, m => m.MatchType, true),
        "teamsize" => sortValue.Equals("asc")
            ? listExtensions.Sort(matches, m => m.TeamSize, true)
            : listExtensions.Sort(matches, m => m.TeamSize, true),
        "minlevel" => sortValue.Equals("asc")
            ? listExtensions.Sort(matches, m => m.MinLevel, true)
            : listExtensions.Sort(matches, m => m.MinLevel, true),
        "maxlevel" => sortValue.Equals("asc")
            ? listExtensions.Sort(matches, m => m.MaxLevel, true)
            : listExtensions.Sort(matches, m => m.MaxLevel, true),
        _ => matches 
    };

    return matches;
}

}