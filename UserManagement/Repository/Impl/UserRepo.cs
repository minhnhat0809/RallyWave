using AutoMapper;
using Entity;
using UserManagement.DTOs;
using UserManagement.DTOs.UserDto;
using UserManagement.DTOs.UserDto.ViewDto;

namespace UserManagement.Repository.Impl;

using Entity;
using UserManagement.Repository;
using UserManagement.Repository.Impl;

using Entity;
using UserManagement.DTOs.UserDto.ViewDto;
using UserManagement.Repository;
using UserManagement.Repository.Impl;

public class UserRepo(RallywaveContext repositoryContext) : RepositoryBase<User>(repositoryContext), IUserRepo
{
    public async Task<List<UserViewDto>> GetUsers(string? filterField, string? filterValue)
    {
        try
        {
            var users = new List<UserViewDto>();
            switch (filterField?.ToLower())
            {
                case "username":
                    users = await FindByConditionAsync(
                        u => u.UserName.Contains(filterValue!),
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
                            u.Status));
                    break;

                case "email":
                    users = await FindByConditionAsync(
                        u => u.Email.Equals(filterValue!),
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
                            u.Status));
                    break;

                case "phonenumber":
                    if (int.TryParse(filterValue, out var phoneNumber))
                    {
                        users = await FindByConditionAsync(
                            u => u.PhoneNumber == phoneNumber,
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
                                u.Status));
                    }
                    break;

                case "status":
                    if (sbyte.TryParse(filterValue, out var status))
                    {
                        users = await FindByConditionAsync(
                            u => u.Status == status,
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
                                u.Status));
                    }
                    break;
            }

            return users;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<UserViewDto?> GetUserById(int userId)
    {
        try
        {
            return await GetByIdAsync(
                userId,
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
                    u.Status));
        }
        catch (Exception e)
        {
            throw new Exception("An error occurred while retrieving the user: " + e.Message);
        }
    }

    public async Task<UserViewDto> CreateUser(User user)
    {
        try
        {
            await CreateAsync(user);
            return new UserViewDto(
                user.UserId,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Gender,
                user.Dob,
                user.Address,
                user.Province,
                user.Avatar,
                user.Status);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<UserViewDto> UpdateUser(User user)
    {
        try
        {
            await UpdateAsync(user);
            return new UserViewDto(
                user.UserId,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Gender,
                user.Dob,
                user.Address,
                user.Province,
                user.Avatar,
                user.Status);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<UserViewDto> DeleteUser(User user)
    {
        try
        {
            await DeleteAsync(user);
            return new UserViewDto(
                user.UserId,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Gender,
                user.Dob,
                user.Address,
                user.Province,
                user.Avatar,
                user.Status);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}




