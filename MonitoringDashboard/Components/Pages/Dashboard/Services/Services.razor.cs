using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages.Dashboard.Services;

public partial class Services
{
    private List<MonitoredService> _monitoredServices = new();
    
    protected override async Task OnInitializedAsync()
    {
        await UpdateMonitoredServices();
    }
    
    private async Task UpdateMonitoredServices()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        _monitoredServices = await db.MonitoredServices
            .OrderByDescending(m => m.Name)
            .ToListAsync();

        await InvokeAsync(StateHasChanged);
    }
}