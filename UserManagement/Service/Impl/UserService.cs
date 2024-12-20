﻿using AutoMapper;
using Entity;
using UserManagement.DTOs;
using UserManagement.DTOs.FriendDto;
using UserManagement.DTOs.SportUserDto;
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
                    var userList = await _unitOfWork.UserRepo.GetUsers(String.Empty, String.Empty);
                    users = _mapper.Map<List<UserViewDto>>(userList);
                }
                else
                {
                    var userList = await _unitOfWork.UserRepo.GetUsers(filterField, filterValue);
                    users = _mapper.Map<List<UserViewDto>>(userList);
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
                    var userView = _mapper.Map<UserViewDto>(user);
                    
                    responseDto.Result = userView;
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

                var userSports =
                    user.UserSports.ToList();
                //if (userUpdateDto.UserSports != null)
                {
                    var userSportUpdates = userUpdateDto.UserSports.ToList();
                    foreach (var userSport in userSportUpdates)
                    {
                        var existingSport = userSports.FirstOrDefault(us => us.SportId == userSport.SportId);

                        if (existingSport != null)
                        {
                            // Update the sport level for existing sports
                            existingSport.Level = userSport.SportLevel;
                        }
                        else
                        {
                            // Add new sport if it does not exist in the user's profile
                            user.UserSports.Add(new UserSport
                            {
                                SportId = userSport.SportId,
                                Level = userSport.SportLevel,
                                Status = 1,
                                Sport = (await _unitOfWork.SportRepository.GetSportById(userSport.SportId))!
                            });
                        }
                    }
                }
                user = await _unitOfWork.UserRepo.UpdateUser(user);

                responseDto.Result = _mapper.Map<UserViewDto>(user);
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
        
        // Friend Service
        public async Task<ResponseDto> GetAllFriendRequestByProperties(int userId, string filter, string value)
        {
            var responseDto = new ResponseDto(null, "", true, 200);
            try
            {
                var user = await _unitOfWork.UserRepo.GetUserById(userId);
                List<Friendship?> response = null;
                if (user == null)
                {
                    responseDto.IsSucceed = false;
                    responseDto.Message = "There are no users with this id";
                    responseDto.StatusCode = StatusCodes.Status400BadRequest;
                    return responseDto;
                }
                

                if (filter.ToLower() == "friends")
                {
                    response = await _unitOfWork.FriendRepository.GetFriendShipByProperties(userId, "all-friends", null);
                    return new ResponseDto(_mapper.Map<List<FriendShipViewDto>>(response), "Get Friends Successfully", true, StatusCodes.Status200OK);
                }else if (filter.ToLower() == "friends-request")
                {
                    response = await _unitOfWork.FriendRepository.GetFriendShipByProperties(userId, "received-requests",
                        null);
                    return new ResponseDto(_mapper.Map<List<FriendShipViewDto>>(response), "Get Friends Request Successfully", true, StatusCodes.Status200OK);
                }

                responseDto.Result = response;
                responseDto.Message = "Filter string error!";

            }
            catch (Exception e)
            {
                responseDto.IsSucceed = false;
                responseDto.Message = e.Message;
                responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return responseDto;
        }

        public async Task<ResponseDto> CreateFriendRequest(int senderId, int receiverId)
        {
            var responseDto = new ResponseDto(null, "", true, 200);
            try
            {
                var sender = await _unitOfWork.UserRepo.GetUserById(senderId);
                var receiver = await _unitOfWork.UserRepo.GetUserById(receiverId);

                if (sender != null && receiver != null && sender != receiver)
                {
                    var friendShipExist = await _unitOfWork.FriendRepository.GetFriendShip(senderId, receiverId);
                    if(friendShipExist == null)
                    {
                        var friendRequest = new Friendship()
                        {
                            Sender = sender,
                            Receiver = receiver,
                            ReceiverId = receiverId,
                            SenderId = senderId,
                            Level = 0,
                            Status = 0
                        };
                        friendShipExist = await _unitOfWork.FriendRepository.CreateFriendShip(friendRequest);
                        return new ResponseDto(_mapper.Map<FriendShipViewDto>(friendShipExist), "Send Friend Request successfully!", true, StatusCodes.Status200OK);
                    }
                    return new ResponseDto(null, "Friend Request Already Exist!", false, StatusCodes.Status400BadRequest);

                }
                return new ResponseDto(null, "User not found!", false, StatusCodes.Status404NotFound);
            }
            catch (Exception e)
            {
                responseDto.IsSucceed = false;
                responseDto.Message = e.Message;
                responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return responseDto;
        }

        public async Task<ResponseDto> AcceptFriendRequest(int senderId, int receiverId)
        {
            var responseDto = new ResponseDto(null, "", true, 200);
            try
            {
                var sender = await _unitOfWork.UserRepo.GetUserById(senderId);
                var receiver = await _unitOfWork.UserRepo.GetUserById(receiverId);

                if (sender != null && receiver != null)
                {
                    var friendShipExist = await _unitOfWork.FriendRepository.GetFriendShip(senderId, receiverId);
                    if(friendShipExist != null)
                    {
                        if(receiverId == friendShipExist.ReceiverId){
                            if (friendShipExist.Status == 1) throw new Exception("You already be friends");
                            friendShipExist.Status = 1;
                            friendShipExist = await _unitOfWork.FriendRepository.AcceptedFriendShip(friendShipExist);
                            return new ResponseDto(_mapper.Map<FriendShipViewDto>(friendShipExist), "Accept Request successfully!", true,
                                StatusCodes.Status200OK);
                        }
                        return new ResponseDto(null, "You are sent Friend Request, Can not edit!", false, StatusCodes.Status400BadRequest);
                    }
                    return new ResponseDto(null, "Friend Request not found!", false, StatusCodes.Status400BadRequest);

                }
                return new ResponseDto(null, "User not found!", false, StatusCodes.Status404NotFound);
            }
            catch (Exception e)
            {
                responseDto.IsSucceed = false;
                responseDto.Message = e.Message;
                responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return responseDto;
        }

        public async Task<ResponseDto> DenyFriendRequest(int senderId, int receiverId)
        {
            var responseDto = new ResponseDto(null, "", true, 200);
            try
            {
                var sender = await _unitOfWork.UserRepo.GetUserById(senderId);
                var receiver = await _unitOfWork.UserRepo.GetUserById(receiverId);

                if (sender != null && receiver != null)
                {
                    var friendShipExist = await _unitOfWork.FriendRepository.GetFriendShip(senderId, receiverId);
                    
                    if(friendShipExist != null)
                    {
                        // denied requets
                        if(friendShipExist.Status == 0){
                            if (friendShipExist.ReceiverId == receiverId)
                            {
                                friendShipExist = await _unitOfWork.FriendRepository.DeniedFriendShip(friendShipExist);
                                return new ResponseDto(friendShipExist, "Denied Successfully successfully!", true,
                                    StatusCodes.Status200OK);
                            }

                            return new ResponseDto(null, "You are sent Friend Request, Can not edit!", false,
                                StatusCodes.Status400BadRequest);
                        }
                        // removed friend
                        if (friendShipExist.Status == 1)
                        {
                            friendShipExist = await _unitOfWork.FriendRepository.DeniedFriendShip(friendShipExist);
                            return new ResponseDto(_mapper.Map<FriendShipViewDto>(friendShipExist), "Delete Friend Successfully!", true,
                                StatusCodes.Status200OK);
                        }
                    }
                    return new ResponseDto(null, "Friend Request not found!", false, StatusCodes.Status400BadRequest);

                }
                return new ResponseDto(null, "User not found!", false, StatusCodes.Status404NotFound);
            }
            catch (Exception e)
            {
                responseDto.IsSucceed = false;
                responseDto.Message = e.Message;
                responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return responseDto;
        }

        public async Task<ResponseDto> DeleteUserSport(int userId, int sportId)
        {
            var responseDto = new ResponseDto(null, "", true, 200);
            try
            {
                var user = await _unitOfWork.UserRepo.GetUserById(userId);
                if (user == null)
                {
                    responseDto.IsSucceed = false;
                    responseDto.Message = "There are no users with this id";
                    responseDto.StatusCode = StatusCodes.Status400BadRequest;
                    return responseDto;
                }

                var userSports =
                    user.UserSports.ToList();
                foreach (var sport in userSports)
                {
                    var sportExist = await _unitOfWork.SportRepository.GetSportById(sportId);
                    if(sportExist != null && sportId == sport.SportId){
                        user.UserSports.Remove(sport);
                        // sport.Status = 0; 
                        // update user sport
                    }
                }
                user = await _unitOfWork.UserRepo.UpdateUser(user);

                responseDto.Result = _mapper.Map<UserViewDto>(user);
                responseDto.Message = "Remove successfully!";

            }
            catch (Exception e)
            {
                responseDto.IsSucceed = false;
                responseDto.Message = e.Message;
                responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return responseDto;
        }
    }