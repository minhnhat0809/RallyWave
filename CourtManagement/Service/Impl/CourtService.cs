using AutoMapper;
using CourtManagement.DTOs;
using CourtManagement.DTOs.CourtDto;
using CourtManagement.DTOs.CourtDto.ViewDto;
using CourtManagement.DTOs.CourtImageDto.ViewDto;
using CourtManagement.Repository;
using CourtManagement.Ultility;
using Entity;

namespace CourtManagement.Service.Impl;

public class CourtService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService, ListExtensions listExtensions) : ICourtService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IImageService _imageService = imageService;
    private const string BucketName = "rallywave";
    
    public async Task<ResponseDto> GetCourts(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber,
        int pageSize)
    {
        var responseDto= new ResponseDto(null, "", true, 200);
        try
        {
            var courts = await _unitOfWork.CourtRepo.GetCourts(filterField, filterValue);

            courts = Sort(courts, sortField, sortValue);

            courts = listExtensions.Paging(courts, pageNumber, pageSize);

            responseDto.Result = courts;
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
            var courtViewDto = await _unitOfWork.CourtRepo.GetByIdAsync(id, c => new CourtViewDto(
                c.CourtId, 
                c.CourtOwner!.Name!,
                c.Sport!.SportName,
                c.CourtName, 
                c.MaxPlayers, 
                c.Address, 
                c.Province, 
                c.Status,
                c.CourtImages.Select(ci => new CourtImageViewDto(ci.ImageId, ci.ImageUrl)).ToList()
                ));
            

            responseDto.Result = courtViewDto;
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
        var responseDto= new ResponseDto(null, "Create successfully", true, StatusCodes.Status201Created);
        
        try
        {
            //overall validation
            responseDto = await ValidateForCreating(courtCreateDto);
            if (responseDto.IsSucceed == false)
            {
                return responseDto;
            }
            
            //map to court 
            var court = _mapper.Map<Court>(courtCreateDto);

            var imagesUrl = new List<string>();

            //check if user uploads images
            if (courtCreateDto.Images is { Count: > 0})
            {
                imagesUrl = await _imageService.UploadImages(courtCreateDto.Images, BucketName, null);
            }

            //add court images
            foreach (var courtImage in imagesUrl.Select(url => new CourtImage
                     {
                         ImageUrl = url,
                         Court = court
                     }))
            {
                court.CourtImages.Add(courtImage);
            }
            
            //create court
            await _unitOfWork.CourtRepo.CreateAsync(court);
            
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

    public async Task<ResponseDto> UpdateCourt(int id, CourtUpdateDto courtUpdateDto)
    {
        var responseDto= new ResponseDto(null, "Update successfully", true, StatusCodes.Status200OK);
        
        try
        {
            //check court in database
            var court = await _unitOfWork.CourtRepo.GetByIdAsync(id, c => c);
            if (court == null)
            {
                responseDto.Message = "There are no courts with this id";
                responseDto.StatusCode = StatusCodes.Status400BadRequest;
                responseDto.IsSucceed = false;
                return responseDto;
            }

            //overall validation
            responseDto = await ValidateForUpdating(courtUpdateDto);
            if (responseDto.IsSucceed == false)
            {
                return responseDto;
            }

            //map to court
            court = _mapper.Map<Court>(courtUpdateDto);
            
            List<string> imagesUrl;

            switch (courtUpdateDto.Images)
            {
                //check if user uploads images
                case { Count: > 0 and < 6 }:
                    imagesUrl = await _imageService.UploadImages(courtUpdateDto.Images, BucketName, null);
                    break;
                default:
                    responseDto.Message = "5 images is max";
                    responseDto.StatusCode = StatusCodes.Status400BadRequest;
                    responseDto.IsSucceed = false;
                    return responseDto;
            }

            //add court images
            foreach (var courtImage in imagesUrl.Select(url => new CourtImage
                     {
                         ImageUrl = url,
                         Court = court
                     }))
            {
                court.CourtImages.Add(courtImage);
            }
            
            //update court
            await _unitOfWork.CourtRepo.UpdateAsync(court);
            
            responseDto.Message = "Update successfully!";
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    public async Task<ResponseDto> DeleteCourt(int id)
    {
        var responseDto= new ResponseDto(null, "Delete successfully", true, StatusCodes.Status200OK);
        
        try
        {
            //check court in database
            var court = await _unitOfWork.CourtRepo.GetByIdAsync(id, c => c);
            if (court == null)
            {
                responseDto.Message = "There are no courts with this id";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                responseDto.IsSucceed = false;
                return responseDto;
            }

            await _unitOfWork.CourtRepo.DeleteAsync(court);
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    public async Task<ResponseDto> DeleteCourtImages(int imageId) 
    {
        var responseDto = new ResponseDto(null, "Delete successfully", true, StatusCodes.Status200OK);
        
        try
        {
            //check court in database
            var courtImage = await _unitOfWork.CourtImageRepo.GetByIdAsync(imageId, ci => ci);
            if (courtImage == null)
            {
                responseDto.Message = "There are no court images with this id";
                responseDto.StatusCode = StatusCodes.Status404NotFound;
                responseDto.IsSucceed = false;
                return responseDto;
            }

            if (Uri.TryCreate(courtImage.ImageUrl, UriKind.Absolute, out var uri))
            {
                // Check if the URL contains the expected bucket name
                if (uri.Host.Contains(BucketName))
                {
                    // Get the part after the bucket URL
                    var path = uri.AbsolutePath;

                    // The S3 key will be everything after the '/' following the bucket name
                    var s3Key = path.StartsWith($"/") ? path.Substring(1) : path;
                    
                    var decodedKey = Uri.UnescapeDataString(s3Key);
                    
                    await _imageService.DeleteImage(BucketName, decodedKey);
                }
            }



            await _unitOfWork.CourtImageRepo.DeleteCourtImage(courtImage);
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.IsSucceed = false;
        }

        return responseDto;
    }

    private async Task<ResponseDto> ValidateForCreating(CourtCreateDto courtCreateDto)
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);

        //check court owner in database
        var existedCourtOwner = await _unitOfWork.CourtOwnerRepo.AnyAsync(co => co.CourtOwnerId == courtCreateDto.CourtOwnerId);

        if (!existedCourtOwner)
        {
            response.IsSucceed = false;
            response.Message = "There are no court owner with this id";
            response.StatusCode = StatusCodes.Status404NotFound;
            return response;
        }

        //check sport in database
        var existedSport = await _unitOfWork.SportRepo.AnyAsync(s => s.SportId == courtCreateDto.SportId);

        if (!existedSport)
        {
            response.IsSucceed = false;
            response.Message = "There are no sports with this id";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }
        return response;
    }
    
    private async Task<ResponseDto> ValidateForUpdating(CourtUpdateDto courtUpdateDto)
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        
        //check sport in database
        var existedSport = await _unitOfWork.SportRepo.AnyAsync(s => s.SportId == courtUpdateDto.SportId);

        if (!existedSport)
        {
            response.IsSucceed = false;
            response.Message = "There are no sports with this id";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }
        
        return response;
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