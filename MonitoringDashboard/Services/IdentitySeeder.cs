using Microsoft.AspNetCore.Identity;
using MonitoringDashboard.Components.Shared.Enums;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Services;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(IHost app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = Enum.GetNames<Role>();

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    public static async Task SeedAdminUserAsync(IHost app)
    {
        string adminUsername = Environment.GetEnvironmentVariable("ADMIN_USERNAME") ?? "admin";
        string adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@example.com";
        string? adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

        if (adminPassword == null)
        {
            Console.WriteLine("ADMIN_PASSWORD environment variable is not set. Skipping admin user creation.");
            return;
        }
        
        using var scope = app.Services.CreateScope();
        
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var admins = await userManager.GetUsersInRoleAsync(nameof(Role.Admin));

        if (admins.Count == 0)
        {
            var defaultAdmin = new User
            {
                UserName = adminUsername,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var creation = await userManager.CreateAsync(defaultAdmin, adminPassword);

            if (creation.Succeeded)
            {
                await userManager.AddToRoleAsync(defaultAdmin, nameof(Role.Admin));
                Console.WriteLine("Default admin user created successfully.");
            }
            else
            {
                Console.WriteLine("Failed to create default admin:");
                foreach (var err in creation.Errors)
                    Console.WriteLine(err.Description);
            }
        }
    }
}