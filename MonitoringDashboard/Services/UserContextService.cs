using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MonitoringDashboard.Components.Shared.Enums;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Services;

public class UserContextService(
    AuthenticationStateProvider auth,
    IServiceScopeFactory scopeFactory) : IUserContextService
{
    public async Task<bool> IsLoggedInAsync()
    {
        var authState = await auth.GetAuthenticationStateAsync();
        return authState.User.Identity?.IsAuthenticated ?? false;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var authState = await auth.GetAuthenticationStateAsync();
        var principal = authState.User;

        if (!principal.Identity?.IsAuthenticated ?? true)
            return null;

        using var scope = scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        return await userManager.GetUserAsync(principal);
    }

    public async Task<bool> IsInRoleAsync(Role role)
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return false;

        using var scope = scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
        return await userManager.IsInRoleAsync(user, role.ToString());
    }
}