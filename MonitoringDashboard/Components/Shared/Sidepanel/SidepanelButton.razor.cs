using Microsoft.AspNetCore.Components;

namespace MonitoringDashboard.Components.Shared.Sidepanel;

public partial class SidepanelButton : ComponentBase
{
    [Parameter] public required string Text { get; set; } = string.Empty;
    [Parameter] public string IconPath { get; set; } = string.Empty;
}