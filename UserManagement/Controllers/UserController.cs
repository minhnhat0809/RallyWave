using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;
using UserManagement.DTOs.UserDto;
using UserManagement.DTOs.UserDto.ViewDto;
using UserManagement.Service;

namespace UserManagement.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets a list of users with optional filtering and sorting.
        /// </summary>
        /// <param name="filterField">Field to filter by.</param>
        /// <param name="filterValue">Value to filter by.</param>
        /// <param name="sortField">Field to sort by.</param>
        /// <param name="sortValue">Sort order (asc or desc).</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Number of users per page.</param>
        /// <returns>A ResponseDto containing the list of users.</returns>
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

        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A ResponseDto containing the user data.</returns>
        [HttpGet("{userId:int}")]
        public async Task<ActionResult<ResponseDto>> GetUserById(int userId)
        {
            var response = await _userService.GetUserById(userId);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="userCreateDto">The user data to create.</param>
        /// <returns>A ResponseDto indicating the result of the operation.</returns>
        /*[HttpPost]
        public async Task<ActionResult<ResponseDto>> CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            var response = await _userService.CreateUser(userCreateDto);
            return response.IsSucceed ? CreatedAtAction(nameof(GetUserById), new { userId = response.Result?.ToString() }, response) : BadRequest(response);
        }*/

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="userUpdateDto">The updated user data.</param>
        /// <returns>A ResponseDto indicating the result of the operation.</returns>
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

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A ResponseDto indicating the result of the operation.</returns>
        /*[HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDto>> DeleteUser(int id)
        {
            var response = await _userService.DeleteUser(id);
            return response.IsSucceed ? Ok(response) : BadRequest(response);
        }*/
        
        // FRIEND
        
        // get all friendship
        // or get all friend request
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
