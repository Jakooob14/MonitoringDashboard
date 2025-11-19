using Microsoft.AspNetCore.Components;
using MonitoringDashboard.Services;

namespace MonitoringDashboard.Components.Pages.Auth;

public partial class Login
{
    [SupplyParameterFromForm] private LoginModel Input { get; set; } = new();
    [SupplyParameterFromQuery] public string? ReturnUrl { get; set; }
    
    private List<string> _errors = new();
    
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            Nav.NavigateTo("/dashboard");
        }
    }

    private async Task HandleLogin()
    {
        _errors.Clear();
        
        var user = await UserManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            _errors.Add("Invalid login attempt.");
            return;
        }
        
        var res = await SignInManager.PasswordSignInAsync(user, Input.Password, true, lockoutOnFailure: false);

        if (!res.Succeeded)
        {
            _errors.Add("Invalid login attempt.");
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