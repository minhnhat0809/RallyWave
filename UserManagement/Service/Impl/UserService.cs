using AutoMapper;
using Entity;
using UserManagement.DTOs;
using UserManagement.DTOs.UserDto;
using UserManagement.DTOs.UserDto.ViewDto;
using UserManagement.Repository;
using UserManagement.Ultility;

namespace UserManagement.Service.Impl;

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

        public async Task<ResponseDto> GetUser(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber, int pageSize)
        {
            var responseDto = new ResponseDto(null, "", true, 200);
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
                    var userlist = await unitOfWork.UserRepo.GetUsers(filterField, filterValue);
                    users = mapper.Map<List<UserViewDto>>(userlist);
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



        public async Task<ResponseDto> GetUserById(int userId)
        {
            var responseDto = new ResponseDto(null, "", true, StatusCodes.Status200OK);
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

        public async Task<ResponseDto> CreateUser(UserCreateDto userCreateDto)
        {
            var responseDto = new ResponseDto(null, "", true, StatusCodes.Status201Created);
            try
            {
                responseDto = await ValidateForCreating(userCreateDto);
                if (responseDto.IsSucceed == false)
                {
                    return responseDto;
                }

                var user = mapper.Map<User>(userCreateDto);

                await unitOfWork.UserRepo.CreateUser(user);

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

        public async Task<ResponseDto> UpdateUser(int id, UserUpdateDto userUpdateDto)
        {
            var responseDto = new ResponseDto(null, "", true, 200);
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

        public async Task<ResponseDto> DeleteUser(int id)
        {
            var responseDto = new ResponseDto(null, "", true, 200);
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
                    user = await unitOfWork.UserRepo.DeleteUser(userModel);
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

        private async Task<ResponseDto> ValidateForCreating(UserCreateDto userCreateDto)
        {
            var response = new ResponseDto(null, "", true, 200);

            // Add your validation logic here
            // Example: Check if user already exists
            var existingUser = await unitOfWork.UserRepo.GetUsers("email",userCreateDto.Email);
            if (existingUser != null)
            {
                response.IsSucceed = false;
                response.Message = "A user with this email already exists.";
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            }

            return response;
        }

        private async Task<ResponseDto> ValidateForUpdating(int id, UserUpdateDto userUpdateDto)
        {
            var response = new ResponseDto(null, "", true, 200);

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