using Entity;

namespace CourtManagement.Repository.Impl;

public class UnitOfWork(RallyWaveContext context) : IUnitOfWork
{
    private readonly RallyWaveContext _context = context;
    public ICourtRepo CourtRepo { get; } = new CourtRepo(context);

    public ICourtOwnerRepo CourtOwnerRepo { get; } = new CourtOwnerRepo(context);

    public ISportRepo SportRepo { get; } = new SportRepo(context);

    public ICourtImageRepo CourtImageRepo { get; } = new CourtImageRepo(context);
}