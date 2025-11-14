using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MonitoringDashboard.Components.Shared.User;

public partial class UserForm
{
    /// <summary>
    /// Identificator used for editing a user also indicates whether the form is in edit mode.
    /// </summary>
    [Parameter] public string? EditUserId { get; set; }

    [SupplyParameterFromForm] public Data.Models.User NewUser { get; set; } = new();

    private string _password = string.Empty;
    private string _selectedRole = "None";
    
    private List<string> _roles = new() { "None" };
    
    private List<string> _errors = new();

    protected override async Task OnParametersSetAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        
        // If we are in edit mode, load the user data
        if (EditUserId != null)
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Data.Models.User>>();
            var user = userManager.FindByIdAsync(EditUserId).GetAwaiter().GetResult();
            if (user != null)
                NewUser = user;
            
            var userRoles = await userManager.GetRolesAsync(NewUser);
            if (userRoles.Count > 0)
                _selectedRole = userRoles[0];
        }
        
        // Load roles
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = await roleManager.Roles.ToListAsync();
        _roles.AddRange(roles.Select(r => r.Name!));
    }

    private async Task HandleUser()
    {
        using var scope = ScopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Data.Models.User>>();

        IdentityResult result;
        
        // Create or update user
        if (EditUserId != null)
        {
            var dbUser = await userManager.FindByIdAsync(EditUserId);
            if (dbUser == null)
            {
                _errors.Add("User not found.");
                return;
            }
            
            dbUser.UserName = NewUser.UserName;
            dbUser.Email = NewUser.Email;

            result = await userManager.UpdateAsync(dbUser);
            NewUser = dbUser;
        }
        else
        {
            result = await userManager.CreateAsync(NewUser, _password);
        }
        
        if (!result.Succeeded)
        {
            _errors = result.Errors.Select(e => e.Description).ToList();
            return;
        }

        // Assign role
        var existingRoles = await userManager.GetRolesAsync(NewUser);
        if (existingRoles.Count > 0)
            await userManager.RemoveFromRolesAsync(NewUser, existingRoles);

        if (_selectedRole != "None")
            await userManager.AddToRoleAsync(NewUser, _selectedRole);

        Nav.NavigateTo("/dashboard/users", true);
    }
}