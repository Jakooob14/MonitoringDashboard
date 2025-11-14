using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages.Dashboard.Users;

public partial class Users
{
    private List<User> _users = new();
    private bool _deleteModalOpen;
    private User? _userToDelete;
    private Dictionary<string, string> _roles = new();

    protected override async Task OnInitializedAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        using var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<User>>();

        if (userStore is IUserEmailStore<User> emailStore)
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var users = userManager.Users;
            _users = await users.ToListAsync();

            foreach (var user in _users)
            {
                var roles = await userManager.GetRolesAsync(user);
                _roles[user.Id] = string.Join(", ", roles);
            }
        }
    }

    private void OpenDeleteModal(User user)
    {
        _userToDelete = user;
        _deleteModalOpen = true;
    }

    private async Task HandleDeleteConfirm()
    {
        if (_userToDelete == null)
            return;

        using var scope = ScopeFactory.CreateScope();
        using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var roles = await userManager.GetRolesAsync(_userToDelete);
        if (roles.Contains("Admin"))
            return;

        if (_users.Count > 0)
        {
            await userManager.DeleteAsync(_userToDelete);
            _users.Remove(_userToDelete);
            await InvokeAsync(StateHasChanged);
        }
    }
}