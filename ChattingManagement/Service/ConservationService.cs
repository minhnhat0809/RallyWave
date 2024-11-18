using AutoMapper;
using ChattingManagement.DTOs;
using ChattingManagement.DTOs.ConservationDto;
using ChattingManagement.DTOs.ConservationDto.ViewDto;
using ChattingManagement.Repository;
using ChattingManagement.Ultility;
using Entity;

namespace ChattingManagement.Service;

public interface IConservationService
{
    Task<ResponseDto?> GetConservationByProperties(string? properties, string? propertiesValue);
    Task<ResponseDto> GetConservationsByProperties(string? properties, string? propertiesValue);
    Task<ResponseDto> CreateConservationAsync(ConservationCreateDto createDto);
    Task<ResponseDto> UpdateConservationAsync(ConservationUpdateDto updateDto);
    Task<ResponseDto> DeleteConservationAsync(int conservationId);
    Task<ResponseDto> DeleteUserOfConservation(int conservationId, int userId);
    Task<ResponseDto> AddUserToConservation(int conservationId, int userId);
}
public class ConservationService : IConservationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly Validate _validate;
    private readonly ListExtensions _listExtensions;

    public ConservationService(IUnitOfWork unitOfWork, IMapper mapper, Validate validate, ListExtensions listExtensions)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validate = validate;
        _listExtensions = listExtensions;
    }
    public async Task<ResponseDto?> GetConservationByProperties(string? properties, string? propertiesValue)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            // Check if the properties argument is empty or null
            if (string.IsNullOrEmpty(properties))
            {
                var conservation = await _unitOfWork.ConservationRepo
                    .GetConservationByProperties(string.Empty, string.Empty);
                return new ResponseDto(_mapper.Map<ConservationViewDto>(conservation),
                    "Get Conservation Successfully", true, StatusCodes.Status200OK);
            }
            else
            {
                // Fetch conservation based on dynamic property and value
                var conservation = await _unitOfWork.ConservationRepo
                    .GetConservationByProperties(properties, propertiesValue);
                
                // Handle case where conservation is not found
                if (conservation == null)
                {
                    return new ResponseDto(null, "No Conservation found with specified properties.", false, StatusCodes.Status404NotFound);
                }
                
                return new ResponseDto(_mapper.Map<ConservationViewDto>(conservation),
                    "Get Conservation Successfully", true, StatusCodes.Status200OK);
            }
        }
        catch (ArgumentException ex)
        {
            // Property does not exist on Conservation entity
            responseDto.Message = $"The property '{properties}' does not exist on the Conservation entity." + ex.Message;
            responseDto.IsSucceed = false;
            responseDto.StatusCode = StatusCodes.Status400BadRequest;
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.IsSucceed = false;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
        }
        return responseDto;
    }

    public async Task<ResponseDto> GetConservationsByProperties(string? properties, string? propertiesValue)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            // Check if the properties argument is empty or null
            if (string.IsNullOrEmpty(properties))
            {
                var conservations = await _unitOfWork.ConservationRepo
                    .GetConservationsByProperties(string.Empty, string.Empty);
                if (conservations == null || !conservations.Any())
                {
                    return new ResponseDto(null, "No Conservations found with specified properties.", false, StatusCodes.Status404NotFound);
                }
                return new ResponseDto(_mapper.Map<List<ConservationViewDto>>(conservations),
                    "Get Conservations Successfully", true, StatusCodes.Status200OK);
            }
            else
            {
                // Fetch conservations based on dynamic property and value
                var conservations = await _unitOfWork.ConservationRepo
                    .GetConservationsByProperties(properties, propertiesValue);
                
                // Handle case where no conservations are found
                if (conservations == null || !conservations.Any())
                {
                    return new ResponseDto(null, "No Conservations found with specified properties.", false, StatusCodes.Status404NotFound);
                }
                
                return new ResponseDto(_mapper.Map<List<ConservationViewDto>>(conservations),
                    "Get Conservations Successfully", true, StatusCodes.Status200OK);
            }
        }
        catch (ArgumentException ex)
        {
            // Property does not exist on Conservation entity
            responseDto.Message = $"The property '{properties}' does not exist on the Conservation entity." + ex.Message;
            responseDto.IsSucceed = false;
            responseDto.StatusCode = StatusCodes.Status400BadRequest;
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.IsSucceed = false;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
        }
        return responseDto;
    }


    public async Task<ResponseDto> CreateConservationAsync(ConservationCreateDto createDto)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            
            /*if(!string.IsNullOrEmpty(createDto.TeamId.ToString()) || createDto.TeamId != null){
                // create conservation for team
                var conservation =
                    await _unitOfWork.ConservationRepo.GetConservationByProperties("TeamId",
                        createDto.ConservationId.ToString());
                if (conservation == null)
                {
                    var team = await _unitOfWork.TeamRepository.GetTeamById(createDto.TeamId);
                    
                    if (team != null)
                        conservation = new Conservation()
                        {
                            ConservationName = createDto.ConservationName,
                            TeamId = team.TeamId,
                            Team = team,
                            Status = 1,
                            Users = 
                        }
                }
                return new ResponseDto(null, "Conservation with this Team was existed", false,
                    StatusCodes.Status500InternalServerError);
                
            }else if (!string.IsNullOrEmpty(createDto.MatchId.ToString()) || createDto.MatchId != null)
            {
                var conservation =
                    await _unitOfWork.ConservationRepo.GetConservationByProperties("MatchId",
                        createDto.ConservationId.ToString());
                if (conservation == null)
                {
                    // create conservation for match
                }

                return new ResponseDto(null, "Conservation with this Match was existed", false,
                    StatusCodes.Status500InternalServerError);

                return new ResponseDto(null, "Conservation with this Team was existed", false,
                    StatusCodes.Status500InternalServerError);
            } return new ResponseDto(null, "Conservation cant create with team and match at same time", false,
                StatusCodes.Status500InternalServerError);*/
           
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.IsSucceed = false;
            responseDto.StatusCode = 500;
        }
        return responseDto;
    }

   

    public async Task<ResponseDto> UpdateConservationAsync(ConservationUpdateDto updateDto)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            var conservation =
                await _unitOfWork.ConservationRepo.GetConservationByProperties("ConservationId", updateDto.ConservationId.ToString());
            if (conservation != null)
            {
                conservation.ConservationName = updateDto.ConservationName;
                var userInConservation = conservation.Users.Select(x=>x.UserId).ToList();
                var userInUpdate = updateDto.Users.Select(x=> x!.UserId).ToList();
                // update user in conservation
                foreach (var userId in userInUpdate)
                {
                    if (!userInConservation.Contains(userId))
                    {
                        var newUser = await _unitOfWork.UserRepository.GetUserById(userId);
                        if (newUser != null) conservation.Users.Add(newUser);
                    }
                }

                conservation = await _unitOfWork.ConservationRepo.UpdateConservationAsync(conservation);
                return new ResponseDto(_mapper.Map<ConservationViewDto>(conservation), "Update Successful!", true, StatusCodes.Status200OK);
            }

            return new ResponseDto(null, "Conservation not found!", false, StatusCodes.Status404NotFound);
           
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.IsSucceed = false;
            responseDto.StatusCode = 500;
        }
        return responseDto;
    }

    public async Task<ResponseDto> DeleteConservationAsync(int conservationId)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            var conservation =
                await _unitOfWork.ConservationRepo.GetConservationByProperties("id", conservationId.ToString());
            if (conservation != null)
            {
                conservation.Status = 0;
                conservation = await _unitOfWork.ConservationRepo.UpdateConservationAsync(conservation);
                return new ResponseDto(_mapper.Map<ConservationViewDto>(conservation), "Delete Successful!", true, StatusCodes.Status200OK);
            }

            return new ResponseDto(null, "Conservation not found!", false, StatusCodes.Status404NotFound);

        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.IsSucceed = false;
            responseDto.StatusCode = 500;
        }
        return responseDto;
    }

    public async Task<ResponseDto> DeleteUserOfConservation(int conservationId, int userId)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            var conservation =
                await _unitOfWork.ConservationRepo.GetConservationByProperties("id", conservationId.ToString());
            if (conservation != null)
            {
                if(conservation.Users.Select(x=>x.UserId).Contains(userId)){
                    var user = await _unitOfWork.UserRepository.GetUserById(userId);
                    if (user != null)
                    {
                        conservation.Users.Remove(user);
                        conservation = await _unitOfWork.ConservationRepo.UpdateConservationAsync(conservation);
                        return new ResponseDto(_mapper.Map<ConservationViewDto>(conservation), "Remove User out of conservation Successful!",
                            true, StatusCodes.Status200OK);
                    }

                    return new ResponseDto(null, "User Not Found!", false, StatusCodes.Status404NotFound);
                } 
                return new ResponseDto(null, "User Not Found in this conservation!", false, StatusCodes.Status500InternalServerError);

            }

            return new ResponseDto(null, "Conservation not found!", false, StatusCodes.Status404NotFound);

        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.IsSucceed = false;
            responseDto.StatusCode = 500;
        }
        return responseDto;
    }
    public async Task<ResponseDto> AddUserToConservation(int conservationId, int userId)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            var conservation =
                await _unitOfWork.ConservationRepo.GetConservationByProperties("id", conservationId.ToString());
            if (conservation != null)
            {
                if(!conservation.Users.Select(x=>x.UserId).Contains(userId)){
                    var user = await _unitOfWork.UserRepository.GetUserById(userId);
                    if (user != null)
                    {
                        conservation.Users.Add(user);
                        conservation = await _unitOfWork.ConservationRepo.UpdateConservationAsync(conservation);
                        return new ResponseDto(_mapper.Map<ConservationViewDto>(conservation), "Add User Successful!",
                            true, StatusCodes.Status200OK);
                    }

                    return new ResponseDto(null, "User Not Found!", false, StatusCodes.Status404NotFound);
                } 
                return new ResponseDto(null, "User Already in this conservation!", false, StatusCodes.Status500InternalServerError);

            }

            return new ResponseDto(null, "Conservation not found!", false, StatusCodes.Status404NotFound);

        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.IsSucceed = false;
            responseDto.StatusCode = 500;
        }
        return responseDto;
    }
}