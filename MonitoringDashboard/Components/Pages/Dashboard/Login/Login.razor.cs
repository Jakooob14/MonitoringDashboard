using MonitoringDashboard.Services;

namespace MonitoringDashboard.Components.Pages.Dashboard.Login;

public partial class Login
{
    private LoginModel _user = new();
    
    private async Task HandleLogin()
    {
        using var scope = ScopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        
        var user = await userService.GetUserByUsernameAsync(_user.Username);
        if (user == null) return;
        
        Console.WriteLine(user.Email);
        
        await Task.CompletedTask;
    }
    
    private class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}