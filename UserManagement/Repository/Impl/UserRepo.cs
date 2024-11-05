using AutoMapper;
using Entity;
using Microsoft.EntityFrameworkCore;
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

public class UserRepo(RallyWaveContext repositoryContext) : RepositoryBase<User>(repositoryContext), IUserRepo
{
    // Get users with optional filtering
    public async Task<List<User>> GetUsers(string? filterField, string? filterValue)
    {
        // If no filter is provided, return all users
        if (string.IsNullOrEmpty(filterField) || string.IsNullOrEmpty(filterValue))
        {
            return await repositoryContext.Users
                .Include(us => us.UserSports)
                .ThenInclude(s => s.Sport).ToListAsync();
        }

        // Handle dynamic filtering based on the provided filterField and filterValue
        // For simplicity, I will assume we're filtering by 'Username' or 'Email'
        if (filterField.Equals("Username", StringComparison.OrdinalIgnoreCase))
        {
            return await repositoryContext.Users
                .Include(us => us.UserSports)
                .ThenInclude(s => s.Sport)
                .Where(u => u.UserName.Contains(filterValue)).ToListAsync();
        }
        if (filterField.Equals("Email", StringComparison.OrdinalIgnoreCase))
        {
            return await repositoryContext.Users
                .Include(us => us.UserSports)
                .ThenInclude(s => s.Sport)
                .Where(u => u.Email != null && u.Email.Contains(filterValue)).ToListAsync();
        }
        else return await repositoryContext.Users
            .Include(us => us.UserSports)
            .ThenInclude(s => s.Sport).ToListAsync();
    }

    // Get a user by ID
    public async Task<User?> GetUserById(int userId)
    {
        return await repositoryContext.Users
            .Include(us => us.UserSports)
            .ThenInclude(s => s.Sport)
            .FirstOrDefaultAsync(u => u.UserId.Equals(userId));
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

    public async Task<List<User>> GetUsersByIds(List<int> userIds)
    {
        return await repositoryContext.Users
            .Include(us=>us.UserSports)
            .ThenInclude(s=>s.Sport)
            .Where(u => userIds.Contains(u.UserId))
            .ToListAsync();
    }

    public async Task<List<User>> GetUnverifiedUsersOlderThanOneDay()
    {
        var cutoffDate = DateTime.Now - TimeSpan.FromMinutes(1);
    
        return await repositoryContext.Users
            .Where(u => u.IsTwoFactorEnabled == 0 && u.CreatedDate <= cutoffDate)
            .ToListAsync();
    }
}




