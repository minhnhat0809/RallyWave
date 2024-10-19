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
    // Get users with optional filtering
    public async Task<List<User>> GetUsers(string? filterField, string? filterValue)
    {
        // If no filter is provided, return all users
        if (string.IsNullOrEmpty(filterField) || string.IsNullOrEmpty(filterValue))
        {
            return await FindAllAsync(u => u);
        }

        // Handle dynamic filtering based on the provided filterField and filterValue
        // For simplicity, I will assume we're filtering by 'Username' or 'Email'
        if (filterField.Equals("Username", StringComparison.OrdinalIgnoreCase))
        {
            return await FindByConditionAsync(u => u.UserName.Contains(filterValue), u => u);
        }
        else if (filterField.Equals("Email", StringComparison.OrdinalIgnoreCase))
        {
            return await FindByConditionAsync(u => u.Email.Contains(filterValue), u => u);
        }
        else
        {
            throw new ArgumentException("Invalid filter field");
        }
    }

    // Get a user by ID
    public async Task<User?> GetUserById(int userId)
    {
        return await GetByIdAsync(userId, u => u);
    }

    // Create a new user
    public async Task<User> CreateUser(User user)
    {
        var isCreated = await CreateAsync(user);
        if (isCreated)
        {
            return user;
        }
        throw new Exception("Failed to create user.");
    }

    // Update an existing user
    public async Task<User> UpdateUser(User user)
    {
        var isUpdated = await UpdateAsync(user);
        if (isUpdated)
        {
            return user;
        }
        throw new Exception("Failed to update user.");
    }

    // Delete a user
    public async Task<User> DeleteUser(User user)
    {
        var isDeleted = await DeleteAsync(user);
        if (isDeleted)
        {
            return user;
        }
        throw new Exception("Failed to delete user.");
    }
}




