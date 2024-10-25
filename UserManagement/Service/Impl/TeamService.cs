using AutoMapper;
using Entity;
using UserManagement.DTOs;
using UserManagement.DTOs.TeamDto;
using UserManagement.DTOs.UserDto;
using UserManagement.DTOs.UserDto.ViewDto;
using UserManagement.Repository;
using UserManagement.Ultility;

namespace UserManagement.Service.Impl;

public interface ITeamService
{
    Task<ResponseDto> GetTeam(string? filterField,
        string? filterValue,
        string? sortField,
        string sortValue,
        int pageNumber,
        int pageSize);

    Task<ResponseDto> GetTeamById(int id);

    Task<ResponseDto> CreateTeam(TeamCreateDto teamCreateDto);

    Task<ResponseDto> UpdateTeam(int id, TeamUpdateDto teamUpdateDto);

    Task<ResponseDto> DeleteTeam(int id);
    // add user to team including team's conservation
    Task<ResponseDto> AddUserToTeam(int teamId, int userId);
    // delete user to team including team's conservation
    Task<ResponseDto> DeleteUserToTeam(int teamId, int userId);
}

public class TeamService : ITeamService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly Validate _validate;
    private readonly ListExtensions _listExtensions;

    public TeamService(IUnitOfWork unitOfWork, IMapper mapper, Validate validate, ListExtensions listExtensions)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validate = validate;
        _listExtensions = listExtensions;
    }

    public async Task<ResponseDto> GetTeam(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber, int pageSize)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            List<TeamViewDto>? teams;

            if (_validate.IsEmptyOrWhiteSpace(filterField) || _validate.IsEmptyOrWhiteSpace(filterValue))
            {
                var teamList = await _unitOfWork.TeamRepository.GetTeams(filterField, filterValue);
                teams = _mapper.Map<List<TeamViewDto>>(teamList);
            }
            else
            {
                var teamList = await _unitOfWork.TeamRepository.GetTeams(filterField, filterValue);
                teams = _mapper.Map<List<TeamViewDto>>(teamList);
            }

            teams = Sort(teams, sortField, sortValue);
            teams = _listExtensions.Paging(teams, pageNumber, pageSize);

            responseDto.Result = teams;
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

    private List<TeamViewDto>? Sort(List<TeamViewDto>? teams, string? sortField, string? sortValue)
    {
        if (teams == null || teams.Count == 0 || string.IsNullOrEmpty(sortField) || string.IsNullOrEmpty(sortValue))
        {
            return teams;
        }

        teams = sortField.ToLower() switch
        {
            "teamname" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? _listExtensions.Sort(teams, t => t.TeamName, true)
                : _listExtensions.Sort(teams, t => t.TeamName, false),
            "status" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? _listExtensions.Sort(teams, t => t.Status, true)
                : _listExtensions.Sort(teams, t => t.Status, false),
            _ => teams
        };

        return teams;
    }

    public async Task<ResponseDto> GetTeamById(int teamId)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            var team = await _unitOfWork.TeamRepository.GetTeamById(teamId);
            if (team == null)
            {
                responseDto.Message = "There are no teams with this id";
                responseDto.StatusCode = 400;
            }
            else
            {
                responseDto.Result = _mapper.Map<TeamViewDto>(team);
                responseDto.Message = "Get successfully!";
            }
        }
        catch (Exception e)
        {
            responseDto.IsSucceed = false;
            responseDto.Message = e.Message;
            responseDto.StatusCode = 500;
        }

        return responseDto;
    }
   
    public async Task<ResponseDto> UpdateTeam(int id, TeamUpdateDto teamUpdateDto)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            var team = await _unitOfWork.TeamRepository.GetTeamById(id);
            if (team == null)
            {
                responseDto.IsSucceed = false;
                responseDto.Message = "There are no teams with this id";
                responseDto.StatusCode = 400;
                return responseDto;
            }

            var teamModel = _mapper.Map<Team>(teamUpdateDto);
            teamModel.TeamId = team.TeamId;
            responseDto.Result = await _unitOfWork.TeamRepository.UpdateTeam(teamModel);
            responseDto.Message = "Update successfully!";
        }
        catch (Exception e)
        {
            responseDto.IsSucceed = false;
            responseDto.Message = e.Message;
            responseDto.StatusCode = 500;
        }

        return responseDto;
    }

    public async Task<ResponseDto> DeleteTeam(int id)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            var team = await _unitOfWork.TeamRepository.GetTeamById(id);
            if (team == null)
            {
                responseDto.IsSucceed = false;
                responseDto.Message = "There are no teams with this id";
                responseDto.StatusCode = 400;
            }
            else
            {
                team.Status = 0;
                await _unitOfWork.TeamRepository.DeleteTeam(team);
                responseDto.Message = "Delete successfully!";
            }
        }
        catch (Exception e)
        {
            responseDto.IsSucceed = false;
            responseDto.Message = e.Message;
            responseDto.StatusCode = 500;
        }

        return responseDto;
    }
    public async Task<ResponseDto> CreateTeam(TeamCreateDto teamCreateDto)
    {
        var responseDto = new ResponseDto(null, "", true, 201);
        List<User> listUserInTeam = new List<User>();
        List<UserTeam> userTeamInTeam = new List<UserTeam>();

        try
        {
            // Get sport and creating user
            var sport = await _unitOfWork.SportRepository.GetSportById(teamCreateDto.SportId);
            var user = await _unitOfWork.UserRepo.GetUserById(teamCreateDto.CreateBy);

            if (sport != null && user != null)
            {
                var team = new Team()
                {
                    TeamName = teamCreateDto.TeamName,
                    Status = 1,
                    SportId = sport.SportId,
                    CreateBy = user.UserId,
                    Sport = sport,
                    CreateByNavigation = user,
                };
                var userIds = teamCreateDto.UserTeams.Select(x => x.UserId).ToList();
                var users = await _unitOfWork.UserRepo.GetUsersByIds(userIds); // team's users
                var validUsers = users.Where(u => u.UserId != user.UserId).ToList(); // without created

                if (validUsers.Count > 0)
                {
                    foreach (var u in validUsers)
                    {
                        // Add user for the conversation
                        listUserInTeam.Add(u);

                        userTeamInTeam.Add(new UserTeam
                        {
                            UserId = u.UserId,
                            User = u,
                            Status = 1
                        });
                    }

                    // Add the creating user to the conversation and team members
                    listUserInTeam.Add(user);
                    userTeamInTeam.Add(new UserTeam()
                    {
                        UserId = user.UserId,
                        Status = 1,
                        User = user
                    });

                    team.UserTeams = userTeamInTeam;

                    // Create a conversation associated with the team
                    var conservation = new Conservation()
                    {
                        ConservationName = team.TeamName,
                        Status = 1,
                        Users = listUserInTeam
                    };

                    team.Conservation = conservation;

                    // Set team size if it's not specified
                    if (teamCreateDto.TeamSize == null)
                        team.TeamSize = (sbyte)listUserInTeam.Count;
                }
                else
                    team.TeamSize = 1;
                
                // Save team to database
                team = await _unitOfWork.TeamRepository.CreateTeam(team);
                await _unitOfWork.SaveChangesAsync(); // Save changes to persist entities

                // Set response
                responseDto.Message = "Create successfully!";
                responseDto.Result = _mapper.Map<TeamViewDto>(team);
            }
        }
        catch (Exception e)
        {
            responseDto.IsSucceed = false;
            responseDto.Message = e.Message;
            responseDto.StatusCode = 500;
        }

        return responseDto;
    }
    public async Task<ResponseDto> DeleteUserToTeam(int teamId, int userId)
    {
        var responseDto = new ResponseDto(null, "", true, 200);

        try
        {
            var userTeam = await _unitOfWork.UserTeamRepository.GetUserTeamByTeamIdAndUserId(teamId, userId);
            if (userTeam == null) throw new Exception("User not found in team!");

            await _unitOfWork.UserTeamRepository.DeleteUserTeam(userTeam);
            await _unitOfWork.SaveChangesAsync();

            responseDto.Message = "User removed from team successfully!";
        }
        catch (Exception e)
        {
            responseDto.IsSucceed = false;
            responseDto.Message = e.Message;
            responseDto.StatusCode = 500;
        }
        return responseDto;
    }
    public async Task<ResponseDto> AddUserToTeam(int teamId, int userId)
    {
        var responseDto = new ResponseDto(null, "", true, 201);
        try
        {
            var user = await _unitOfWork.UserRepo.GetUserById(userId);
            var team = await _unitOfWork.TeamRepository.GetTeamById(teamId);
            
            if (user != null && team != null)
            {
                var creator = await _unitOfWork.UserRepo.GetUserById(team.CreateBy);
                if (team.UserTeams.Any(ut => ut.UserId == userId) || team.CreateBy == userId)
                    throw new Exception("User already in this team!");
                    
                // Team's Users
                team.UserTeams.Add(new UserTeam { UserId = userId, User = user, Status = 1 });
                if(team.UserTeams.Count == 1) team.UserTeams.Add(new UserTeam { UserId = creator!.UserId, User = creator!, Status = 1 });
                
                // Team's Conservation
                if (team.Conservation != null)
                {
                    team.Conservation.Users.Add(user);
                }else
                {
                    team.Conservation = new Conservation
                    {
                        ConservationName = team.TeamName,
                        Status = 1,
                        Users = [user, creator]
                    };
                }
                // Set Size
                team.TeamSize = (team.UserTeams.Count > team.TeamSize) 
                    ? (sbyte)(team.TeamSize + 1) 
                    : team.TeamSize;

                team = await _unitOfWork.TeamRepository.UpdateTeam(team);
                await _unitOfWork.SaveChangesAsync();
                
                responseDto.Message = "Add User to Team successfully!";
                responseDto.Result = _mapper.Map<TeamViewDto>(team);
            }
        }
        catch (Exception e)
        {
            responseDto.IsSucceed = false;
            responseDto.Message = e.Message;
            responseDto.StatusCode = 500;
        }

        return responseDto;
    }

    
}
