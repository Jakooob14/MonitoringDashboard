using Microsoft.AspNetCore.Components;

namespace MonitoringDashboard.Components.Pages.Dashboard.Services;

public partial class AddMonitoredService
{
    [SupplyParameterFromForm] private Data.Models.MonitoredService NewMonitoredService { get; set; } = new();

    private async Task AddService()
    {
        NewMonitoredService.CreatedAt = DateTime.UtcNow;
        NewMonitoredService.LastDowntimeAt = DateTime.UtcNow;

        Db.MonitoredServices.Add(NewMonitoredService);
        await Db.SaveChangesAsync();
        NewMonitoredService = new();
    }
}