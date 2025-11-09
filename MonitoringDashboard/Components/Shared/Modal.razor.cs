using Microsoft.AspNetCore.Components;

namespace MonitoringDashboard.Components.Shared;

public partial class Modal : ComponentBase
{
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }
    [Parameter] public string Title { get; set; } = "Modal";
    [Parameter] public bool ShowFooter { get; set; } = true;
    [Parameter] public EventCallback OnConfirm { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string ConfirmButtonText { get; set; } = "Confirm";
    [Parameter] public string ConfirmButtonClass { get; set; } = string.Empty;
    [Parameter] public string CancelButtonText { get; set; } = "Cancel";
    [Parameter] public string CancelButtonClass { get; set; } = string.Empty;

    private async Task Close()
    {
        IsOpen = false;
        await IsOpenChanged.InvokeAsync(IsOpen);
    }

    private async Task Confirm()
    {
        if (OnConfirm.HasDelegate) await OnConfirm.InvokeAsync();
        await Close();
    }
}