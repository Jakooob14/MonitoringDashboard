using Microsoft.AspNetCore.Components;
using MonitoringDashboard.Data;

namespace MonitoringDashboard.Components.Shared.Maintenance;

public partial class MaintenanceCardBase : ComponentBase
{
    [Parameter] public required Data.Models.Maintenance Maintenance { get; set; }
    [Inject] public IServiceScopeFactory ScopeFactory { get; set; } = null!;
    [Inject] public NavigationManager Nav { get; set; } = null!;
}