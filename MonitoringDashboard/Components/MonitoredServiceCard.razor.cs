using Microsoft.AspNetCore.Components;

namespace MonitoringDashboard.Components;

public partial class MonitoredServiceCard : ComponentBase
{
    [Parameter] public required Data.Models.MonitoredService MonitoredService { get; set; }
}