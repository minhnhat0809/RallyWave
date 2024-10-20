﻿using Entity;

namespace BookingManagement.Repository.Impl;

public class UserRepo(RallyWaveContext repositoryContext) : RepositoryBase<User>(repositoryContext), IUserRepo
{
    
}