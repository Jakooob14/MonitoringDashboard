using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MonitoringDashboard.Components.Shared.Enums;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Services;

public class UserContextService(
    AuthenticationStateProvider auth,
    UserManager<User> userManager) : IUserContextService
{
    public async Task<bool> IsLoggedInAsync()
    {
        var authState = await auth.GetAuthenticationStateAsync();
        return authState.User.Identity?.IsAuthenticated ?? false;
    }
    
    public async Task<User?> GetCurrentUserAsync()
    {
        var authState = await auth.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity == null) return null;

        if (user.Identity.IsAuthenticated)
            return await userManager.GetUserAsync(user);

        return null;
    }
    
    public async Task<bool> IsInRoleAsync(Role role)
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return false;

        return await userManager.IsInRoleAsync(user, role.ToString());
    }
}