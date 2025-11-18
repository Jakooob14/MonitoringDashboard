namespace MonitoringDashboard.Components.Shared.Sidepanel;

public partial class Sidepanel
{
    private Data.Models.User? _currentUser;
    private bool _sidebarOpen;

    private List<ContextMenu.ContextMenu.ContextMenuItem> _contextMenuItems = new()
    {
        new ContextMenu.ContextMenu.ContextMenuItem
        {
            Text = "Profile",
            IconPath = "icons/user.svg",
            Href = "/dashboard/profile"
        },
        new ContextMenu.ContextMenu.ContextMenuItem
        {
            Text = "Logout",
            IconPath = "icons/logout.svg",
            Href = "/logout",
            ForceLoad = true
        }
    };

    private bool _isContextMenuOpen;

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await UserContextService.GetCurrentUserAsync();
    }
}