using Entity;

namespace ChattingManagement.Repository.Impl;

public class UnitOfWork(RallyWaveContext context) : IUnitOfWork
{
    public IMessageRepo MessageRepo { get; } = new MessageRepo(context);
    public IConservationRepo ConservationRepo { get; } = new ConservationRepo(context);
    public IUserRepository UserRepository { get; } = new UserRepository(context);
    public ITeamRepository TeamRepository { get; } = new TeamRepository(context);
}