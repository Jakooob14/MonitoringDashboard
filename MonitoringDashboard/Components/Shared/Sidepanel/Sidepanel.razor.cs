using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MonitoringDashboard.Components.Shared.Sidepanel;

public partial class Sidepanel
{
    private Data.Models.User? _currentUser;

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
            Href = "/logout"
        }
    };

    private bool _isContextMenuOpen;

    protected override async Task OnInitializedAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Data.Models.User>>();
        
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity == null) return;

        if (user.Identity.IsAuthenticated)
        {
            _currentUser = await userManager.GetUserAsync(user);
        }
    }
}