using AutoMapper;
using CourtManagement.DTOs;
using CourtManagement.DTOs.CourtDto;
using CourtManagement.DTOs.CourtDto.ViewDto;
using CourtManagement.Repository;
using CourtManagement.Ultility;
using Entity;

namespace CourtManagement.Service.Impl;

public class CourtService(IUnitOfWork unitOfWork, IMapper mapper, ListExtensions listExtensions) : ICourtService
{
    public async Task<ResponseDto> GetCourts(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber,
        int pageSize)
    {
        var responseDto= new ResponseDto(null, "", true, 200);
        try
        {
            var courts = await unitOfWork.courtRepo.GetCourts(filterField, filterValue);

            courts = Sort(courts, sortField, sortValue);

            courts = listExtensions.Paging(courts, pageNumber, pageSize);

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

    private List<CourtsViewDto>? Sort(List<CourtsViewDto>? courts, string? sortField, string? sortValue)
    {
        if (courts == null || courts.Count == 0 || string.IsNullOrEmpty(sortField) || 
            string.IsNullOrEmpty(sortValue) || string.IsNullOrWhiteSpace(sortField) || string.IsNullOrWhiteSpace(sortValue))
        {
            return courts;
        }

        courts = sortField.ToLower() switch
        {
            "courtname" => sortValue.Equals("asc")
                ? listExtensions.Sort(courts, c => c.CourtName, true)
                : listExtensions.Sort(courts, c => c.CourtName, false),
            "address" => sortValue.Equals("asc")
                ? listExtensions.Sort(courts, c => c.Address, true)
                : listExtensions.Sort(courts, c => c.Address, false),
            "province" => sortValue.Equals("asc")
                ? listExtensions.Sort(courts, c => c.Province, true)
                : listExtensions.Sort(courts, c => c.Province, false),
            "status" => sortValue.Equals("asc")
                ? listExtensions.Sort(courts, c => c.Status, true)
                : listExtensions.Sort(courts, c => c.Status, false),
            _ => courts
        };

        return courts;
    }
}