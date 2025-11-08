using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace MonitoringDashboard.Components.Shared.Sidepanel;

public partial class SidepanelButton : ComponentBase, IDisposable
{
    [Parameter] public required string Text { get; set; } = string.Empty;
    [Parameter] public string IconPath { get; set; } = string.Empty;
    [Parameter] public string Href { get; set; } = "#";
        
    private EventHandler<LocationChangedEventArgs>? _locationChangedHandler;

    private bool _active;

    public void Dispose()
    {
        if (_locationChangedHandler != null) Nav.LocationChanged -= _locationChangedHandler;
    }
    
    protected override void OnInitialized()
    {
        _locationChangedHandler = (_, __) => StateHasChanged();
        Nav.LocationChanged += _locationChangedHandler;

        _active = IsActive();
    }
    
    private bool IsActive()
    {
        var currentPath = Nav.ToBaseRelativePath(Nav.Uri)
            .TrimEnd('/')
            .ToLowerInvariant();

        var targetPath = Href.TrimStart('/')
            .TrimEnd('/')
            .ToLowerInvariant();

        return currentPath.Contains(targetPath) ||
               (string.IsNullOrEmpty(currentPath) && string.IsNullOrEmpty(targetPath));
    }
}