using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages;

public partial class Home
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
        _monitoredServices = await Db.MonitoredServices.OrderByDescending(m => m.Name)
            .Include(m => m.Checks.OrderByDescending(c => c.CheckedAt).Take(5))
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
}