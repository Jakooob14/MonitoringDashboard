using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Services;

public interface IAuthService
{
    Task<User?> GetUserByUsernameAsync(string username);
    
    Task<bool> VerifyPasswordAsync(User user, string password);
    
    Task CreateUserAsync(string username, string email, string password);
}