using Microsoft.AspNetCore.Identity;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Helpers;

public static class UserHelpers
{
    public static async Task<bool> IsAdminAsync(User user, IServiceScopeFactory scopeFactory)
    {
        using var scope = scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
        var roles = await userManager.GetRolesAsync(user);
        return roles.Contains("Admin");
    }
}