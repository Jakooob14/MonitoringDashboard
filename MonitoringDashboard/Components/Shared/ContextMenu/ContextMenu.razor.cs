using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MonitoringDashboard.Components.Shared.ContextMenu;

public partial class ContextMenu : ComponentBase
{

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
    [Parameter] public List<ContextMenuItem> Items { get; set; } = new();

    [Parameter] public EventCallback OnClose { get; set; }

    public class ContextMenuItem
    {
        public string Text { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public Action? OnClick { get; set; }
        public string? Href { get; set; }
        public bool Disabled { get; set; }
        public bool ForceLoad { get; set; }
    }
    
    private async Task CloseMenu(MouseEventArgs e)
    {
        await OnClose.InvokeAsync(null);
        await Task.Delay(50);
        await JS.InvokeVoidAsync("simulateClick", e.ClientX, e.ClientY);
    }
    
    private async Task HandleItemClick(ContextMenuItem item)
    {
        if (item.Disabled) return;

        if (item.Href != null && item.ForceLoad)
        {
            Nav.NavigateTo(item.Href, true);
            return;
        }
        
        await OnClose.InvokeAsync(null);
        item.OnClick?.Invoke();
    }
}
