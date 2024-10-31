using AutoMapper;
using Entity;
using Identity.API.BusinessObjects;
using Identity.API.BusinessObjects.UserViewModel;
using Identity.API.Repository;
using Identity.API.Ultility;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Services;

public interface IUserService
{
    Task<ResponseModel> GetUser(string? filterField,
        string? filterValue,
        string? sortField,
        string sortValue,
        int pageNumber,
        int pageSize);

    Task<ResponseModel> GetUserById(int userId);

    Task<ResponseModel> CreateUser(UserCreateDto userCreateDto);

    Task<ResponseModel> UpdateUser(int id, UserUpdateDto userCreateDto);

    Task<ResponseModel> DeleteUser(int id);
    Task<ResponseModel> GetUserByEmailAsync(string email);
}
public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly Validate validate;
        private readonly ListExtensions listExtensions;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, Validate validate, ListExtensions listExtensions)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.validate = validate;
            this.listExtensions = listExtensions;
        }

        public async Task<ResponseModel> GetUser(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber, int pageSize)
        {
            var responseDto = new ResponseModel(null, "", true, 200);
            try
            {
                List<UserViewDto>? users;

                if (validate.IsEmptyOrWhiteSpace(filterField) || validate.IsEmptyOrWhiteSpace(filterValue))
                {
                    users = await unitOfWork.UserRepo.FindAllAsync(
                        u => new UserViewDto(
                            u.UserId,
                            u.UserName,
                            u.Email,
                            u.PhoneNumber,
                            u.Gender,
                            u.Dob,
                            u.Address,
                            u.Province,
                            u.Avatar,
                            u.Status,
                            u.CreatedDate), null);
                }
                else
                {
                    users = mapper.Map<List<UserViewDto>>(await unitOfWork.UserRepo.GetUsers(filterField, filterValue));
                }

                users = Sort(users, sortField, sortValue);
                users = listExtensions.Paging(users, pageNumber, pageSize);

                responseDto.Result = users;
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

        private List<UserViewDto>? Sort(List<UserViewDto>? users, string? sortField, string? sortValue)
        {
            if (users == null || users.Count == 0 || string.IsNullOrEmpty(sortField) || 
                string.IsNullOrEmpty(sortValue) || string.IsNullOrWhiteSpace(sortField) || string.IsNullOrWhiteSpace(sortValue))
            {
                return users;
            }

            users = sortField.ToLower() switch
            {
                "username" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? listExtensions.Sort(users, u => u.UserName, true)
                    : listExtensions.Sort(users, u => u.UserName, false),
                "email" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? listExtensions.Sort(users, u => u.Email, true)
                    : listExtensions.Sort(users, u => u.Email, false),
                "phonenumber" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? listExtensions.Sort(users, u => u.PhoneNumber, true)
                    : listExtensions.Sort(users, u => u.PhoneNumber, false),
                "gender" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? listExtensions.Sort(users, u => u.Gender, true)
                    : listExtensions.Sort(users, u => u.Gender, false),
                "dob" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? listExtensions.Sort(users, u => u.Dob, true)
                    : listExtensions.Sort(users, u => u.Dob, false),
                "status" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? listExtensions.Sort(users, u => u.Status, true)
                    : listExtensions.Sort(users, u => u.Status, false),
                _ => users
            };

            return users;
        }



        public async Task<ResponseModel> GetUserById(int userId)
        {
            var responseDto = new ResponseModel(null, "", true, StatusCodes.Status200OK);
            try
            {
                var user = await unitOfWork.UserRepo.GetUserById(userId);
                if (user == null)
                {
                    responseDto.Message = "There are no users with this id";
                    responseDto.StatusCode = StatusCodes.Status400BadRequest;
                }
                else
                {
                    responseDto.Result = user;
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

        public async Task<ResponseModel> CreateUser(UserCreateDto userCreateDto)
        {
            var responseDto = new ResponseModel(null, "", true, StatusCodes.Status201Created);
            try
            {
                responseDto = await ValidateForCreating(userCreateDto);
                if (responseDto.IsSucceed == false)
                {
                    return responseDto;
                }

                var user = mapper.Map<User>(userCreateDto);

                responseDto.Result = await unitOfWork.UserRepo.CreateUser(user);

                responseDto.Message = "Create successfully!";
            }
            catch (Exception e)
            {
                responseDto.IsSucceed = false;
                responseDto.Message = e.Message;
                responseDto.StatusCode = 500;
            }

            return responseDto;
        }

        public async Task<ResponseModel> UpdateUser(int id, UserUpdateDto userUpdateDto)
        {
            var responseDto = new ResponseModel(null, "", true, 200);
            try
            {
                var user = await unitOfWork.UserRepo.GetUserById(id);
                if (user == null)
                {
                    responseDto.IsSucceed = false;
                    responseDto.Message = "There are no users with this id";
                    responseDto.StatusCode = StatusCodes.Status400BadRequest;
                    return responseDto;
                }

                responseDto = await ValidateForUpdating(id, userUpdateDto);

                if (responseDto.IsSucceed == false)
                {
                    return responseDto;
                }

                var userModel = mapper.Map<User>(userUpdateDto);
                userModel.UserId = user.UserId;
            
                responseDto.Result = await unitOfWork.UserRepo.UpdateUser(userModel);
                responseDto.Message = "Update successfully!";

            }
            catch (Exception e)
            {
                responseDto.IsSucceed = false;
                responseDto.Message = e.Message;
                responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return responseDto;
        }

        public async Task<ResponseModel> DeleteUser(int id)
        {
            var responseDto = new ResponseModel(null, "", true, 200);
            try
            {
                var user = await unitOfWork.UserRepo.GetUserById(id);
                if (user == null)
                {
                    responseDto.IsSucceed = false;
                    responseDto.Message = "There are no users with this id";
                    responseDto.StatusCode = StatusCodes.Status400BadRequest;
                }
                else
                {
                    User userModel = mapper.Map<User>(user);
                    await unitOfWork.UserRepo.DeleteUser(userModel);
                    responseDto.Result = user;
                    responseDto.Message = "Delete successfully!";
                }
            }
            catch (Exception e)
            {
                responseDto.IsSucceed = false;
                responseDto.Message = e.Message;
                responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return responseDto;
        }

        public async Task<ResponseModel> GetUserByEmailAsync(string email)
        {
            var responseDto = new ResponseModel(null, "", true, StatusCodes.Status200OK);
            try
            {
                var userList = await unitOfWork.UserRepo.FindByConditionAsync(u => u.Email == email,u => u) ;
                if (userList == null || userList.Count == 0 )
                {
                    responseDto.Message = "There are no users with this mail";
                    responseDto.StatusCode = StatusCodes.Status400BadRequest;
                }
                else
                {
                    responseDto.Result = userList;
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

        private async Task<ResponseModel> ValidateForCreating(UserCreateDto userCreateDto)
        {
            var response = new ResponseModel(null, "", true, 200);

            // Add your validation logic here
            // Example: Check if user already exists
            var existingUser = await unitOfWork.UserRepo.GetUsers("email",userCreateDto.Email);
            if (existingUser.Count != 0 || !existingUser.IsNullOrEmpty() )
            {
                response.IsSucceed = false;
                response.Message = "A user with this email already exists.";
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            }

            return response;
        }

        private async Task<ResponseModel> ValidateForUpdating(int id, UserUpdateDto userUpdateDto)
        {
            var response = new ResponseModel(null, "", true, 200);

            // Add your validation logic here
            // Example: Check if user exists
            var existingUser = await unitOfWork.UserRepo.GetUserById(id);
            if (existingUser == null)
            {
                response.IsSucceed = false;
                response.Message = "User not found.";
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            }

            return response;
        }
    }