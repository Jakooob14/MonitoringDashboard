using Microsoft.AspNetCore.Components;
using MonitoringDashboard.Services;

namespace MonitoringDashboard.Components.Pages.Auth;

public partial class Login
{
    [SupplyParameterFromForm] private LoginModel Input { get; set; } = new();
    [SupplyParameterFromQuery] public string? ReturnUrl { get; set; }

    private async Task HandleLogin()
    {
        var user = await UserManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            // TODO: Show error message
            return;
        }
        
        var res = await SignInManager.PasswordSignInAsync(user, Input.Password, true, lockoutOnFailure: false);

        if (!res.Succeeded)
        {
            // TODO: Show error message
            return;
        }
        
        Nav.NavigateTo(ReturnUrl ?? "/dashboard/services", true);
    }
    
    private class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}