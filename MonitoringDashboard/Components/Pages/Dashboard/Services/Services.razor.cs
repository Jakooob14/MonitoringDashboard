using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Components.Shared.Enums;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages.Dashboard.Services;

public partial class Services
{
    private List<MonitoredService> _monitoredServices = new();

    private bool _isAdmin;
    private bool _isUser;
    
    protected override async Task OnInitializedAsync()
    {
        await UpdateMonitoredServices();
        
        _isAdmin = await UserContextService.IsInRoleAsync(Role.Admin);
        _isUser = await UserContextService.IsInRoleAsync(Role.User);
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