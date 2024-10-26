using Entity;

namespace UserManagement.Repository.Impl;

public class UnitOfWork : IUnitOfWork
{
    private readonly RallyWaveContext _context;

    // Constructor to inject the DbContext
    public UnitOfWork(RallyWaveContext context, IUserTeamRepository userTeamRepository)
    {
        _context = context;
        UserTeamRepository = userTeamRepository;

        // Initialize repositories
        UserRepo = new UserRepo(_context);
        TeamRepository = new TeamRepository(_context);
        SportRepository = new SportRepository(_context);
        ConservationRepository = new ConservationRepository(_context);
    }

    // Repository properties
    public IUserRepo UserRepo { get; }
    public ITeamRepository TeamRepository { get; }
    public ISportRepository SportRepository { get; }
    public IConservationRepository ConservationRepository { get; }
    public IUserTeamRepository UserTeamRepository { get; }

    // Save changes to the database
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    // Dispose the context when done
    public void Dispose()
    {
        _context.Dispose();
    }
}