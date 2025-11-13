using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Services;

public class AuthService(
    AppDbContext db,
    IPasswordHasher passwordHasher
    ) : IAuthService
{
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await db.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> VerifyPasswordAsync(User user, string password)
    {
        return passwordHasher.VerifyHashedPassword(user.PasswordHash, password);
    }

    public async Task CreateUserAsync(string username, string email, string password)
    {
        throw new NotImplementedException();
    }
}