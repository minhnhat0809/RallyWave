﻿using Entity;
using Microsoft.EntityFrameworkCore;

namespace ChattingManagement.Repository.Impl;

public interface IUserRepository : IRepositoryBase<User>
{
    Task<User?> GetUserById(int id);
}

public class UserRepository(RallyWaveContext repositoryContext) : RepositoryBase<User>(repositoryContext), IUserRepository
{
    public async Task<User?> GetUserById(int id)
    {
        return await repositoryContext.Users.FirstOrDefaultAsync(x => x.UserId == id);
    }
}