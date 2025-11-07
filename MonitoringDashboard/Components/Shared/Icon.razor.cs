using Microsoft.AspNetCore.Components;

namespace MonitoringDashboard.Components.Shared;

public partial class Icon : ComponentBase
{
    [Parameter] public required string Path { get; set; } = string.Empty;
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
}