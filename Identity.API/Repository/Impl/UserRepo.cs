﻿using Entity;
using Identity.API.BusinessObjects.UserViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Repository.Impl;

public class UserRepo(RallywaveContext repositoryContext) : RepositoryBase<User>(repositoryContext), IUserRepo
{
    private readonly RallywaveContext _repositoryContext = repositoryContext;

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
                        u => u.Email != null && u.Email.Equals(filterValue!),
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

    public async Task<User?> GetUserByPropertyAndValue(string property, string value)
    {
        try
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Property and value must not be null or empty.");
            }

            // Fetch user based on the property name and value
            var user = await _repositoryContext.Users
                .FirstOrDefaultAsync(u => u.Email == value);

            // Return the found user or null if not found
            return user;
        }
        catch (ArgumentException ex)
        {
            // Handle invalid arguments
            throw new Exception(ex.Message);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw new Exception($"An error occurred while retrieving the user: {ex.Message}");
        }
    }

}




