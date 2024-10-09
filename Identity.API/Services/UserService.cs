using Entity;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Services;

public interface IUserService
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> AddUserAsync(User user); 
}
public class UserService : IUserService
{
    private readonly RallywaveContext _context; 
    public UserService(RallywaveContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x=> x.Email == email);
    }

    public async Task<User?> AddUserAsync(User user)
    {
        
        User? newUser = await _context.Users.FirstOrDefaultAsync(x=>x.Email == user.Email);
        if (newUser == null)
        {
            _context.Users.Add(user);;
            await _context.SaveChangesAsync();  
        }

        return user;
    }
}