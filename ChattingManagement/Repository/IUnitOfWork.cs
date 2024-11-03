using ChattingManagement.Repository.Impl;

namespace ChattingManagement.Repository;

public interface IUnitOfWork
{
    IMessageRepo MessageRepo { get; }
    IUserRepository UserRepository { get; }
    IConservationRepo ConservationRepo { get; }
    ITeamRepository TeamRepository { get; }
}