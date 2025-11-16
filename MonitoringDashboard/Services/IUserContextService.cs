using MonitoringDashboard.Components.Shared.Enums;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Services;

public interface IUserContextService
{
    Task<bool> IsLoggedInAsync();
    Task<User?> GetCurrentUserAsync();
    Task<bool> IsInRoleAsync(Role role);
}