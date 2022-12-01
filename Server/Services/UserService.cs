
using MailApp.Models;
using MailApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MailApp.Services;

public class UserService
{
    private ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetUsers(string? username)
    {
        return await _context.Users.Where((user) => user.UserName.Contains(username ?? string.Empty)).ToListAsync();
    }

    private async Task<User?> GetUser(string username)
    {
        return await _context.Users.Where((user) => user.UserName == username).FirstOrDefaultAsync();
    }

    private async Task<User> CreateUser(string username)
    {
        var user = new User()
        {
            UserName = username,
        };
        EntityEntry<User> craetedUser = await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        user.Id = craetedUser.Entity.Id;
        return user;
    }

    public async Task<User> GetUserOrCreate(string username)
    {
        User? user = await GetUser(username);
        if (user is not null)
        {
            return user;
        }
        return await CreateUser(username);
    }
}