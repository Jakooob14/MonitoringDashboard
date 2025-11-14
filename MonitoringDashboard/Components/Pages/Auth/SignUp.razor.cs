using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages.Auth;

public partial class SignUp
{
    private SignupModal _formData = new();
    
    private async Task HandleSignup()
    {
        var form = new MultipartFormDataContent();
        form.Add(new StringContent(_formData.Username), "username");
        form.Add(new StringContent(_formData.Email), "email");
        form.Add(new StringContent(_formData.Password), "password");

        User user = new();
        await UserStore.SetUserNameAsync(user, _formData.Username, CancellationToken.None);
        IUserEmailStore<User> emailStore = (IUserEmailStore<User>)UserStore;
        await emailStore.SetEmailAsync(user, _formData.Email, CancellationToken.None);
        var res = await UserManager.CreateAsync(user, _formData.Password);
        Console.WriteLine(res);

        if (!res.Succeeded)
        {
            // TODO: Show error message
            return;
        }
        
        Nav.NavigateTo("/", true);
    }
    
    private class SignupModal
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}