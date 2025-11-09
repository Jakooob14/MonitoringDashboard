using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MonitoringDashboard.Components.Shared;

public partial class ContextMenu : ComponentBase
{

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
    [Parameter] public List<ContextMenuItem> Items { get; set; } = new();

    [Parameter] public EventCallback OnClose { get; set; }

    public class ContextMenuItem
    {
        public string Text { get; set; } = string.Empty;
        public Action? OnClick { get; set; }
        public bool Disabled { get; set; }
    }
    
    private async Task CloseMenu(MouseEventArgs e)
    {
        await OnClose.InvokeAsync(null);
        await Task.Delay(50);
        await JS.InvokeVoidAsync("simulateClick", e.ClientX, e.ClientY);
    }
}
