using Microsoft.AspNetCore.Components;
using MonitoringDashboard.Components.Shared;
using MonitoringDashboard.Components.Shared.ContextMenu;

namespace MonitoringDashboard.Components.Pages.Dashboard.Services;

public partial class MonitoredServiceDashboardCard
{
    [Parameter] public EventCallback OnDeleted { get; set; }
    private bool _deleteModalOpen;
    
    private async Task HandleDeleteConfirm()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();
        
        db.MonitoredServices.Remove(MonitoredService);
        await db.SaveChangesAsync();

        if (OnDeleted.HasDelegate) await OnDeleted.InvokeAsync();
    }
}