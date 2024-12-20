﻿using Entity;
using Identity.API.Repository.Impl;

namespace Identity.API.Repository;

public interface IUnitOfWork 
{ 
    IUserRepo UserRepo { get; }
    IAuthRepository AuthRepository { get; }
    ICourtOwnerRepository CourtOwnerRepository { get; }
    IFirebaseStorageRepository FirebaseStorageRepository { get; }
}
