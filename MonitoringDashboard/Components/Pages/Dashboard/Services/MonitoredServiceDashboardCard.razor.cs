using Microsoft.AspNetCore.Components;
using MonitoringDashboard.Components.Shared;
using MonitoringDashboard.Components.Shared.ContextMenu;

namespace MonitoringDashboard.Components.Pages.Dashboard.Services;

public partial class MonitoredServiceDashboardCard
{
    [Parameter] public EventCallback OnDeleted { get; set; }
    
    private bool _contextMenuOpen;
    private List<ContextMenu.ContextMenuItem> _contextMenuItems = [];
    private bool _deleteModalOpen;

    protected override void OnInitialized()
    {
        _contextMenuItems =
        [
            new ContextMenu.ContextMenuItem
            {
                Text = "Edit",
                OnClick = () => Nav.NavigateTo($"/dashboard/services/edit/{MonitoredService.Id}"),
                IconPath = "icons/pen.svg"
            },
            new ContextMenu.ContextMenuItem
            {
                Text = "Delete",
                IconPath = "icons/trash-can.svg",
                OnClick = () => _deleteModalOpen = true
            }
        ];
    }

    private void ToggleContextMenu()
    {
        _contextMenuOpen = !_contextMenuOpen;
    }
    
    private async Task HandleDeleteConfirm()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();
        
        db.MonitoredServices.Remove(MonitoredService);
        await db.SaveChangesAsync();

        if (OnDeleted.HasDelegate) await OnDeleted.InvokeAsync();
    }
}