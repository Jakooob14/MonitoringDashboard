using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages;

public partial class Home
{
    private List<MonitoredService> _monitoredServices = new();
    private List<Maintenance> _maintenances = new();
    
    private int _failedServicesCount;
    
    protected override async Task OnInitializedAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        await UpdateMonitoredServices(db);
        await UpdateMaintenances(db);
        _failedServicesCount = await GetFailedServicesCount(db);
    }

    private async Task UpdateMonitoredServices(AppDbContext? db = null)
    {
        if (db == null)
        {
            using var scope = ScopeFactory.CreateScope();
            await using var scopedDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db = scopedDb;
        }
        
        _monitoredServices = await db.MonitoredServices
            .OrderByDescending(m => m.Name)
            .ToListAsync();
    }
    
    private async Task<int> GetFailedServicesCount(AppDbContext? db = null)
    {
        int count = 0;
        
        if (db == null)
        {
            using var scope = ScopeFactory.CreateScope();
            await using var scopedDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db = scopedDb;
        }

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
    
    private async Task UpdateMaintenances(AppDbContext? db = null)
    {
        if (db == null)
        {
            using var scope = ScopeFactory.CreateScope();
            await using var scopedDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db = scopedDb;
        }
        
        _maintenances = await db.Maintenances
            .Where(m => m.Status == MaintenanceStatus.Scheduled || m.Status == MaintenanceStatus.InProgress)
            .OrderByDescending(m => m.Severity)
            .ToListAsync();
    }
}