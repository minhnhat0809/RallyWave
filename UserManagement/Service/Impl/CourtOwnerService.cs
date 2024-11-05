using AutoMapper;
using UserManagement.DTOs;
using UserManagement.DTOs.CourtOwnerDto.ViewDto;
using UserManagement.Repository;
using UserManagement.Ultility;

namespace UserManagement.Service.Impl;

public interface ICourtOwnerService
{
    Task<ResponseDto> GetAllCourtOwnersAsync(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber, int pageSize);
    Task<ResponseDto> GetCourtOwnerByIdAsync(int id);
}
public class CourtOwnerService : ICourtOwnerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly Validate _validate;
    private readonly ListExtensions _listExtensions;

    public CourtOwnerService(IUnitOfWork unitOfWork, IMapper mapper, Validate validate, ListExtensions listExtensions)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validate = validate;
        _listExtensions = listExtensions;
    }
    
    
    public async Task<ResponseDto> GetCourtOwnerByIdAsync(int id)
    {
        var responseDto = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {
            var courtOwner = await _unitOfWork.CourtOwnerRepository.GetCourtOwnerById(id);
            if (courtOwner == null)
            {
                responseDto.Message = "There are no Court Owner with this id";
                responseDto.StatusCode = StatusCodes.Status400BadRequest;
            }
            else
            {
                responseDto.Result = courtOwner;
                responseDto.Message = "Get successfully!";
            }
        }
        catch (Exception e)
        {
            responseDto.IsSucceed = false;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.Message = e.Message;
        }

        return responseDto;
    }
    public async Task<ResponseDto> GetAllCourtOwnersAsync(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber, int pageSize)
        {
            var responseDto = new ResponseDto(null, "", true, 200);
            try
            {
                List<CourtOwnerViewDto>? courtOwners;

                if (_validate.IsEmptyOrWhiteSpace(filterField) || _validate.IsEmptyOrWhiteSpace(filterValue))
                {
                    courtOwners = await _unitOfWork.CourtOwnerRepository.FindAllAsync(
                        u => new CourtOwnerViewDto(
                            u.CourtOwnerId,
                            u.Name,
                            u.Email,
                            u.PhoneNumber,  // Ensure correct data type
                            u.Gender,
                            u.Dob,  // Make sure DateOnly is handled properly
                            u.Address,
                            u.Province,
                            u.Avatar,
                            u.Status),
                        null);
                }
                else
                {
                    var courtList = await _unitOfWork.CourtOwnerRepository.GetCourtOwners(filterField, filterValue);
                    courtOwners = _mapper.Map<List<CourtOwnerViewDto>>(courtList);
                }

                courtOwners = Sort(courtOwners, sortField, sortValue);
                courtOwners = _listExtensions.Paging(courtOwners, pageNumber, pageSize);

                responseDto.Result = courtOwners;
                responseDto.Message = "Get successfully!";
            }
            catch (Exception e)
            {
                responseDto.Message = e.Message;
                responseDto.IsSucceed = false;
                responseDto.StatusCode = 500;
            }
            return responseDto;
        }

        private List<CourtOwnerViewDto>? Sort(List<CourtOwnerViewDto>? courtOwners, string? sortField, string? sortValue)
        {
            if (courtOwners == null || courtOwners.Count == 0 || string.IsNullOrEmpty(sortField) || 
                string.IsNullOrEmpty(sortValue) || string.IsNullOrWhiteSpace(sortField) || string.IsNullOrWhiteSpace(sortValue))
            {
                return courtOwners;
            }

            courtOwners = sortField.ToLower() switch
            {
                "name" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? _listExtensions.Sort(courtOwners, u => u.Name, true)
                    : _listExtensions.Sort(courtOwners, u => u.Name, false),
                "email" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? _listExtensions.Sort(courtOwners, u => u.Email, true)
                    : _listExtensions.Sort(courtOwners, u => u.Email, false),
                "phonenumber" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? _listExtensions.Sort(courtOwners, u => u.PhoneNumber, true)
                    : _listExtensions.Sort(courtOwners, u => u.PhoneNumber, false),
                "gender" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? _listExtensions.Sort(courtOwners, u => u.Gender, true)
                    : _listExtensions.Sort(courtOwners, u => u.Gender, false),
                "dob" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? _listExtensions.Sort(courtOwners, u => u.Dob, true)
                    : _listExtensions.Sort(courtOwners, u => u.Dob, false),
                "status" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? _listExtensions.Sort(courtOwners, u => u.Status, true)
                    : _listExtensions.Sort(courtOwners, u => u.Status, false),
                _ => courtOwners
            };

            return courtOwners;
        }


}