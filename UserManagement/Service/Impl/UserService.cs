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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Validate _validate;
        private readonly ListExtensions _listExtensions;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, Validate validate, ListExtensions listExtensions)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validate = validate;
            _listExtensions = listExtensions;
        }

        public async Task<ResponseDto> GetUser(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber, int pageSize)
        {
            var responseDto = new ResponseDto(null, "", true, 200);
            try
            {
                List<UserViewDto>? users;

                if (_validate.IsEmptyOrWhiteSpace(filterField) || _validate.IsEmptyOrWhiteSpace(filterValue))
                {
                    users = await _unitOfWork.UserRepo.FindAllAsync(
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
                    var userlist = await _unitOfWork.UserRepo.GetUsers(filterField, filterValue);
                    users = _mapper.Map<List<UserViewDto>>(userlist);
                }

                users = Sort(users, sortField, sortValue);
                users = _listExtensions.Paging(users, pageNumber, pageSize);

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
                    ? _listExtensions.Sort(users, u => u.UserName, true)
                    : _listExtensions.Sort(users, u => u.UserName, false),
                "email" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? _listExtensions.Sort(users, u => u.Email, true)
                    : _listExtensions.Sort(users, u => u.Email, false),
                "phonenumber" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? _listExtensions.Sort(users, u => u.PhoneNumber, true)
                    : _listExtensions.Sort(users, u => u.PhoneNumber, false),
                "gender" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? _listExtensions.Sort(users, u => u.Gender, true)
                    : _listExtensions.Sort(users, u => u.Gender, false),
                "dob" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? _listExtensions.Sort(users, u => u.Dob, true)
                    : _listExtensions.Sort(users, u => u.Dob, false),
                "status" => sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? _listExtensions.Sort(users, u => u.Status, true)
                    : _listExtensions.Sort(users, u => u.Status, false),
                _ => users
            };

            return users;
        }



        public async Task<ResponseDto> GetUserById(int userId)
        {
            var responseDto = new ResponseDto(null, "", true, StatusCodes.Status200OK);
            try
            {
                var user = await _unitOfWork.UserRepo.GetUserById(userId);
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

                var user = _mapper.Map<User>(userCreateDto);

                await _unitOfWork.UserRepo.CreateUser(user);

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
                var user = await _unitOfWork.UserRepo.GetUserById(id);
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

                user.UserName = userUpdateDto.UserName;
                user.Address = userUpdateDto.Address;
                user.Dob = userUpdateDto.Dob;
                user.Avatar = user.Avatar;
                user.Gender = userUpdateDto.Gender;
                user.Province = userUpdateDto.Province;
                
                responseDto.Result = await _unitOfWork.UserRepo.UpdateUser(user);
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
                var user = await _unitOfWork.UserRepo.GetUserById(id);
                if (user == null)
                {
                    responseDto.IsSucceed = false;
                    responseDto.Message = "There are no users with this id";
                    responseDto.StatusCode = StatusCodes.Status400BadRequest;
                }
                else
                {
                    User userModel = _mapper.Map<User>(user);
                    userModel.Status = 0;
                    user = await _unitOfWork.UserRepo.DeleteUser(userModel);
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
            var existingUser = await _unitOfWork.UserRepo.GetUsers("email",userCreateDto.Email);
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
            var existingUser = await _unitOfWork.UserRepo.GetUserById(id);
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