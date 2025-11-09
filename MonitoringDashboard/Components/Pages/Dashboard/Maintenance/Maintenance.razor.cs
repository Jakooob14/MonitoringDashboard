using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages.Dashboard.Maintenance;

public partial class Maintenance
{
    private List<Data.Models.Maintenance> _maintenances = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadMaintenances();
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