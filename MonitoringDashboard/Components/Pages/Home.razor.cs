using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages;

public partial class Home
{
    private List<MonitoredService> _monitoredServices = new();
    
    private int _failedServicesCount;
    
    protected override async Task OnInitializedAsync()
    {
        await UpdateMonitoredServices();
        _failedServicesCount = await GetFailedServicesCount();
    }

    private async Task UpdateMonitoredServices()
    {
        using var scope = ScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        _monitoredServices = await db.MonitoredServices
            .OrderByDescending(m => m.Name)
            .ToListAsync();
    }
    
    private async Task<int> GetFailedServicesCount()
    {
        int count = 0;
        
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        foreach (var monitoredService in _monitoredServices)
        {
            var lastCheck = db.ServiceChecks
                .Where(c => c.MonitoredServiceId == monitoredService.Id)
                .OrderByDescending(c => c.CheckedAt)
                .FirstOrDefault();

            if (lastCheck == null || lastCheck.IsSuccessful) continue;

            count++;
        }
        
        return count;
    }
}