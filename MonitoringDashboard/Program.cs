using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MonitoringDashboard.Components;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;
using MonitoringDashboard.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
    
builder.Services.AddCascadingAuthenticationState();

// Disable detailed logging
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
builder.Logging.AddFilter("System.Net.Http.HttpClient.ServiceChecker.LogicalHandler", LogLevel.Warning);
builder.Logging.AddFilter("System.Net.Http.HttpClient.ServiceChecker.ClientHandler", LogLevel.Warning);

Env.Load(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, ".env"));

var connectionString = $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};" +
                       $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};" +
                       $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")};";


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHttpClient<ServiceChecker>();
builder.Services.AddHttpClient("BrowserClient")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler());

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/login";
    options.LogoutPath = "/logout";

    options.SlidingExpiration = true;
});

builder.Services.AddIdentityCore<User>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
});

builder.Services.AddHostedService<MonitoringWorker>();
builder.Services.AddHostedService<CleanupWorker>();

builder.Services.AddScoped<IUserContextService, UserContextService>();

builder.Services.AddControllers();

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();
app.MapAdditionalIdentityEndpoints();

app.UseAntiforgery();

await IdentitySeeder.SeedRolesAsync(app);
await IdentitySeeder.SeedAdminUserAsync(app);

app.Run();