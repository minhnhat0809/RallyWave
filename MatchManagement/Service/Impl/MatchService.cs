using AutoMapper;
using Entity;
using MatchManagement.DTOs;
using MatchManagement.DTOs.MatchDto;
using MatchManagement.DTOs.MatchDto.ViewDto;
using MatchManagement.Repository;
using MatchManagement.Ultility;

namespace MatchManagement.Service.Impl;

public class MatchService(IUnitOfWork unitOfWork, IMapper mapper, Validate validate) : IMatchService
{
    public async Task<ResponseDto> GetMatches(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber,
        int pageSize)
    {
        var responseDto = new ResponseDto(null, "Get successfully", true, StatusCodes.Status200OK);
        try
        {
            List<Match>? matches;
            if (validate.IsEmptyOrWhiteSpace(filterField) || validate.IsEmptyOrWhiteSpace(filterValue))
            {
                matches = await unitOfWork.matchRepo.FindAllAsync(m => m.Booking, m => m.UserMatches, m => m.Conservation, m => m.Sport!);
            }
            else
            {
                matches = await unitOfWork.matchRepo.GetMatches(filterField, filterValue);
            }
            
            matches = Sort(matches, sortField, sortValue);

            matches = Paging(matches, pageNumber, pageSize);

            responseDto.Result = mapper.Map<List<MatchViewDto>>(matches);
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
            var match = await unitOfWork.matchRepo.GetByIdAsync(id, m => m.Booking, m => m.UserMatches, m => m.Conservation, m => m.Sport!);
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

            await unitOfWork.matchRepo.CreateAsync(match);
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
            var match = await unitOfWork.matchRepo.GetByIdAsync(id);
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

            await unitOfWork.matchRepo.CreateAsync(match);
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
            var match = await unitOfWork.matchRepo.GetByIdAsync(id);
            if (match == null)
            {
                responseDto.Message = "There are no matches with this id";
                responseDto.IsSucceed = false;
                responseDto.StatusCode = StatusCodes.Status404NotFound;
            }
            else
            {
                await unitOfWork.matchRepo.DeleteAsync(match);
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
        var responseDto = new ResponseDto(null, "Get successfully", true, StatusCodes.Status201Created);
        try
        {
            
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
        var responseDto = new ResponseDto(null, "Get successfully", true, StatusCodes.Status201Created);
        try
        {
            
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }
    
    private static List<Match>? Paging(List<Match>? matches, int pageNumber, int pageSize)
    {
        if (matches == null || matches.Count == 0)
        {
            return matches;
        }

        matches = matches
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        return matches;
    }

   private static List<Match>? Sort(List<Match>? matches, string? sortField, string? sortValue)
{
    if (matches == null || matches.Count == 0 || string.IsNullOrEmpty(sortField) || 
        string.IsNullOrEmpty(sortValue) || string.IsNullOrWhiteSpace(sortField) || string.IsNullOrWhiteSpace(sortValue))
    {
        return matches;
    }

    matches = sortField.ToLower() switch
    {
        "date" => sortValue.Equals("asc")
            ? matches.OrderBy(b => b.Date).ToList()
            : matches.OrderByDescending(b => b.Date).ToList(),
        "timestart" => sortValue.Equals("asc")
            ? matches.OrderBy(b => b.TimeStart).ToList()
            : matches.OrderByDescending(b => b.TimeStart).ToList(),
        "timeend" => sortValue.Equals("asc")
            ? matches.OrderBy(b => b.TimeEnd).ToList()
            : matches.OrderByDescending(b => b.TimeEnd).ToList(),
        "status" => sortValue.Equals("asc")
            ? matches.OrderBy(b => b.Status).ToList()
            : matches.OrderByDescending(b => b.Status).ToList(),
        "matchtype" => sortValue.Equals("asc")
            ? matches.OrderBy(b => b.MatchType).ToList()
            : matches.OrderByDescending(b => b.MatchType).ToList(),
        "teamsize" => sortValue.Equals("asc")
            ? matches.OrderBy(b => b.TeamSize).ToList()
            : matches.OrderByDescending(b => b.TeamSize).ToList(),
        "minlevel" => sortValue.Equals("asc")
            ? matches.OrderBy(b => b.MinLevel).ToList()
            : matches.OrderByDescending(b => b.MinLevel).ToList(),
        "maxlevel" => sortValue.Equals("asc")
            ? matches.OrderBy(b => b.MaxLevel).ToList()
            : matches.OrderByDescending(b => b.MaxLevel).ToList(),
        "gender" => sortValue.Equals("asc")
            ? matches.OrderBy(b => b.Gender).ToList()
            : matches.OrderByDescending(b => b.Gender).ToList(),
        "minage" => sortValue.Equals("asc")
            ? matches.OrderBy(b => b.MinAge).ToList()
            : matches.OrderByDescending(b => b.MinAge).ToList(),
        "maxage" => sortValue.Equals("asc")
            ? matches.OrderBy(b => b.MaxAge).ToList()
            : matches.OrderByDescending(b => b.MaxAge).ToList(),
        "blockingoff" => sortValue.Equals("asc")
            ? matches.OrderBy(b => b.BlockingOff).ToList()
            : matches.OrderByDescending(b => b.BlockingOff).ToList(),
        "mode" => sortValue.Equals("asc")
            ? matches.OrderBy(b => b.Mode).ToList()
            : matches.OrderByDescending(b => b.Mode).ToList(),
        _ => matches 
    };

    return matches;
}

}