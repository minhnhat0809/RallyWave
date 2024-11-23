using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;
using UserManagement.DTOs.UserDto.ViewDto;
using UserManagement.Service;

namespace UserManagement.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetUsers(
            [FromQuery] string? filterField,
            [FromQuery] string? filterValue,
            [FromQuery] string? sortField,
            [FromQuery] string sortValue = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _userService.GetUser(filterField, filterValue, sortField, sortValue, pageNumber, pageSize);
            return Ok(response);
        }
        [HttpGet("{userId:int}")]
        public async Task<ActionResult<ResponseDto>> GetUserById(int userId)
        {
            var response = await _userService.GetUserById(userId);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ResponseDto>> UpdateUser(int id, [FromBody] UserUpdateDto userUpdateDto)
        {
            var response = await _userService.UpdateUser(id, userUpdateDto);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }
        [HttpDelete("{userId:int}/sport/{sportId:int}")]
        public async Task<ActionResult<ResponseDto>> UpdateUser(int userId, int sportId)
        {
            var response = await _userService.DeleteUserSport(userId, sportId);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{receiverId:int}/friends")]
        public async Task<ActionResult<ResponseDto>> GetAllFriendship(int receiverId)
        {
            var response = await _userService.GetAllFriendRequestByProperties(receiverId,  "friends", String.Empty);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }
        [HttpGet("{receiverId:int}/friend-requests")]
        public async Task<ActionResult<ResponseDto>> GetAllFriendshipRequest(int receiverId)
        {
            var response = await _userService.GetAllFriendRequestByProperties(receiverId,  "friends-request",  String.Empty);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }
        
        [HttpPost("{senderId:int}/friend/{receiverId:int}")]
        public async Task<ActionResult<ResponseDto>> CreateFriendRequest(int senderId, int receiverId)
        {
            var response = await _userService.CreateFriendRequest(senderId, receiverId);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }
        [HttpPut("{receiverId:int}/friend/{senderId:int}")]
        public async Task<ActionResult<ResponseDto>> AcceptFriendRequest(int senderId, int receiverId)
        {
            var response = await _userService.AcceptFriendRequest(senderId, receiverId);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }
        
        [HttpDelete("{receiverId:int}/friend/{senderId:int}")]
        public async Task<ActionResult<ResponseDto>> DenyFriendRequestOrDeleteFriend(int senderId, int receiverId)
        {
            var response = await _userService.DenyFriendRequest(senderId, receiverId);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }
        

    }
}
