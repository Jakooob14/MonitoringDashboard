using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages.Dashboard.Users;

public partial class AddUser : ComponentBase
{
    [SupplyParameterFromForm]
    public User NewUser { get; set; } = new();
    
    private string _password = string.Empty;
    
    private List<string> _errors = new();
    
    private async Task HandleAddUser()
    {
        using var scope = ScopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
        var result = await userManager.CreateAsync(NewUser, _password);
        if (!result.Succeeded)
        {
            _errors = result.Errors.Select(e => e.Description).ToList();
            return;
        }
        
        Nav.NavigateTo("/dashboard/users", true);
    }
}