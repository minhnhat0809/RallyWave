using Entity;

namespace CourtManagement.Repository.Impl;

public class UnitOfWork(RallywaveContext context) : IUnitOfWork
{
    private readonly RallywaveContext _context = context;


    public ICourtRepo courtRepo { get; } = new CourtRepo(context);
}