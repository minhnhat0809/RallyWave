using Entity;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Services;

public interface IUserService
{
    Task<User> GetUserByEmailAsync(string email);
    Task CreateUserAsync(User user);
}
public class UserService : IUserService
{
    private readonly RallywaveContext _context;

    public UserService(RallywaveContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email) 
               ?? throw new InvalidOperationException();
    }

    public async Task CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}