﻿using Identity.API.Repository;

namespace UserManagement.Repository;

public interface IUnitOfWork
{ 
    IUserRepo UserRepo { get; }
}