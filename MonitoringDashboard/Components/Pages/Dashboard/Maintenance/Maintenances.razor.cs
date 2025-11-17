using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Components.Shared.Enums;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages.Dashboard.Maintenance;

public partial class Maintenances
{
    private List<Data.Models.Maintenance> _maintenances = new();

    private bool _isAdmin;
    private bool _isUser;
    

    protected override async Task OnInitializedAsync()
    {
        await LoadMaintenances();

        _isAdmin = await UserContextService.IsInRoleAsync(Role.Admin);
        _isUser = await UserContextService.IsInRoleAsync(Role.User);
    }

    private async Task LoadMaintenances()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        _maintenances = await db.Maintenances
            .Include(m => m.MonitoredServices)
            .OrderByDescending(m => m.StartTime)
            .ToListAsync();
    }
}