using AutoMapper;
using CourtManagement.DTOs;
using CourtManagement.DTOs.CourtDto;
using CourtManagement.DTOs.CourtDto.ViewDto;
using CourtManagement.Repository;
using Entity;

namespace CourtManagement.Service.Impl;

public class CourtService(IUnitOfWork unitOfWork, IMapper mapper) : ICourtService
{
    public async Task<ResponseDto> GetCourts(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber,
        int pageSize)
    {
        var responseDto= new ResponseDto(null, "", true, 200);
        try
        {
            var courts = await unitOfWork.courtRepo.GetCourts(filterField, filterValue);

            courts = Sort(courts, sortField, sortValue);

            courts = Paging(courts, pageNumber, pageSize);

            responseDto.Result = mapper.Map<List<CourtViewDto>>(courts);
            responseDto.Message = "Get successfully!";
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = 500;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    public async Task<ResponseDto> GetCourtById(int id)
    {
        var responseDto= new ResponseDto(null, "", true, 200);
        try
        {
            var court = await unitOfWork.courtRepo.GetCourtById(id);

            responseDto.Result = mapper.Map<CourtViewDto>(court);
            responseDto.Message = "Get successfully";
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = 500;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    public async Task<ResponseDto> CreateCourt(CourtCreateDto courtCreateDto)
    {
        var responseDto= new ResponseDto(null, "", true, 200);
        
        try
        {
            
            responseDto.Message = "Create successfully!";
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = 500;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    public async Task<ResponseDto> UpdateCourt(CourtUpdateDto courtUpdateDto)
    {
        var responseDto= new ResponseDto(null, "", true, 200);
        
        try
        {
            
            responseDto.Message = "Update successfully!";
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = 500;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    public async Task<ResponseDto> DeleteCourt(int id)
    {
        var responseDto= new ResponseDto(null, "", true, 200);
        
        try
        {
            var court = await unitOfWork.courtRepo.GetCourtById(id);
            if (court == null)
            {
                responseDto.Message = "There are no courts with this id";
                responseDto.StatusCode = 401;
                responseDto.IsSucceed = false;
                return responseDto;
            }

            await unitOfWork.courtRepo.DeleteCourt(court);

            responseDto.Message = "Delete successfully!";
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = 500;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    private static List<Court>? Sort(List<Court>? courts, string? sortField, string? sortValue)
    {
        if (courts == null || courts.Count == 0 || string.IsNullOrEmpty(sortField) || 
            string.IsNullOrEmpty(sortValue) || string.IsNullOrWhiteSpace(sortField) || string.IsNullOrWhiteSpace(sortValue))
        {
            return courts;
        }

        courts = sortField.ToLower() switch
        {
            "courtname" => sortValue.Equals("asc")
                ? courts.OrderBy(c => c.CourtName).ToList()
                : courts.OrderByDescending(c => c.CourtName).ToList(),
            "maxplayers" => sortValue.Equals("asc")
                ? courts.OrderBy(c => c.MaxPlayers).ToList()
                : courts.OrderByDescending(c => c.MaxPlayers).ToList(),
            "address" => sortValue.Equals("asc")
                ? courts.OrderBy(c => c.Address).ToList()
                : courts.OrderByDescending(c => c.Address).ToList(),
            "province" => sortValue.Equals("asc")
                ? courts.OrderBy(c => c.Province).ToList()
                : courts.OrderByDescending(c => c.Province).ToList(),
            "status" => sortValue.Equals("asc")
                ? courts.OrderBy(c => c.Status).ToList()
                : courts.OrderByDescending(c => c.Status).ToList(),
            _ => courts
        };

        return courts;
    }

    private static List<Court>? Paging(List<Court>? courts, int pageNumber, int pageSize)
    {
        if (courts == null || courts.Count == 0)
        {
            return courts;
        }
        
        return courts
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageNumber)
            .ToList();
    }
}