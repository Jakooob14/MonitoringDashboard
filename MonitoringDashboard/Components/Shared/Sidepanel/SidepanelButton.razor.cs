using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace MonitoringDashboard.Components.Shared.Sidepanel;

public partial class SidepanelButton : ComponentBase, IDisposable
{
    [Parameter] public required string Text { get; set; } = string.Empty;
    [Parameter] public string IconPath { get; set; } = string.Empty;
    [Parameter] public string Href { get; set; } = "#";
        
    private EventHandler<LocationChangedEventArgs>? _locationChangedHandler;

    public void Dispose()
    {
        if (_locationChangedHandler != null) Nav.LocationChanged -= _locationChangedHandler;
    }
    
    protected override void OnInitialized()
    {
        _locationChangedHandler = (_, __) => StateHasChanged();
        Nav.LocationChanged += _locationChangedHandler;
    }
    
    private string GetClass()
    {
        var uri = Nav.Uri;
        var baseUri = Nav.BaseUri;
        var relativePath = uri.Replace(baseUri, "", StringComparison.OrdinalIgnoreCase);

        // Active if it starts with Href
        bool isActive = relativePath.StartsWith(Href.TrimStart('/'), StringComparison.OrdinalIgnoreCase);

        return isActive ? "bg-zinc-900" : "";
    }
    
    private void OnClick()
    {
        Nav.NavigateTo(Href);
    }
}