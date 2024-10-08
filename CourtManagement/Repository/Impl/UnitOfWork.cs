using Entity;

namespace CourtManagement.Repository.Impl;

public class UnitOfWork(RallywaveContext context) : IUnitOfWork
{
    private readonly RallywaveContext _context = context;
    public ICourtRepo CourtRepo { get; } = new CourtRepo(context);

    public ICourtOwnerRepo CourtOwnerRepo { get; } = new CourtOwnerRepo(context);

    public ISportRepo SportRepo { get; } = new SportRepo(context);
}