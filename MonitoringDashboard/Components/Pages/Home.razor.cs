using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages;

public partial class Home : IAsyncDisposable
{
    private List<MonitoredService> _monitoredServices = new();
    private HubConnection? _hubConnection;
    
    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
    
    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/hubs/monitoring"))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On("ServiceChecked", async () =>
        {
            await UpdateMonitoredServices();
            await InvokeAsync(StateHasChanged);
        });

        await _hubConnection.StartAsync();
        
        await UpdateMonitoredServices();
    }

    private async Task UpdateMonitoredServices()
    {
        using var scope = ScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        _monitoredServices = await db.MonitoredServices
            .OrderByDescending(m => m.Name)
            .ToListAsync();
    }
    
    private async Task CheckServiceNow(MonitoredService service)
    {
        var check = await ServiceChecker.CheckAsync(service);
        
        check.MonitoredServiceId = service.Id;
        
        Db.ServiceChecks.Add(check);
        await Db.SaveChangesAsync();
        
        await UpdateMonitoredServices();
    }
    
    private int GetFailedServicesCount()
    {
        int count = 0;
        
        foreach (var monitoredService in _monitoredServices)
        {
            var lastCheck = Db.ServiceChecks
                .Where(c => c.MonitoredServiceId == monitoredService.Id)
                .OrderByDescending(c => c.CheckedAt)
                .FirstOrDefault();

            if (lastCheck == null || lastCheck.IsSuccessful) continue;

            count++;
        }
        
        return count;
    }
}