using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages.Dashboard.Incidents;

public partial class Incidents
{
    private List<Incident> _incidents = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadIncidents();
    }

    private async Task LoadIncidents()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        _incidents = await db.Incidents
            .OrderByDescending(i => i.StartTime)
            .ToListAsync();
    }
}