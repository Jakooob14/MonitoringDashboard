using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages.Dashboard.Services;

public partial class Service : ComponentBase
{
    [Parameter] public Guid Id { get; set; }
    
    private List<Incident> _serviceIncidents = new();

    protected override async Task OnParametersSetAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        _serviceIncidents = await db.Incidents
            .Include(i => i.MonitoredService)
            .Where(i => i.MonitoredServiceId == Id)
            .OrderByDescending(i => i.StartTime)
            .ToListAsync();

        Console.WriteLine(Id);
    }
}