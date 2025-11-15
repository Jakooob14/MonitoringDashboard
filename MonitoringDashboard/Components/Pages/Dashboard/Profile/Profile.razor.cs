using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages.Dashboard.Profile;

public partial class Profile
{
    [SupplyParameterFromForm] public EditProfileModel ProfileModel { get; set; } = new();
    private List<string> _errors = new();

    protected override async Task OnInitializedAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity == null) return;

        if (user.Identity.IsAuthenticated)
        {
            var res = await userManager.GetUserAsync(user);
            if (res == null) return;

            ProfileModel.Id = res.Id;
            ProfileModel.UserName = res.UserName!;
            ProfileModel.Email = res.Email!;
        }
        
        StateHasChanged();
    }

    private async Task HandleEditProfile()
    {
        using var scope = ScopeFactory.CreateScope();
        using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
        var user = await userManager.FindByIdAsync(ProfileModel.Id);
        
        if (user != null)
        {
            user.UserName = ProfileModel.UserName;
            user.Email = ProfileModel.Email;

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                Nav.Refresh(true);
            }
            else
            {
                _errors = result.Errors.Select(e => e.Description).ToList();
            }
        }
    }
    
    public class EditProfileModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}