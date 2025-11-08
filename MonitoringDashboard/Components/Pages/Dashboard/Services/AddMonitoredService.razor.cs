using Microsoft.AspNetCore.Components;
using MonitoringDashboard.Data;
using MonitoringDashboard.Helpers;

namespace MonitoringDashboard.Components.Pages.Dashboard.Services;

public partial class AddMonitoredService
{
    [SupplyParameterFromForm] private Data.Models.MonitoredService NewMonitoredService { get; set; } = new();

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            SetNameAsRandomPhrase();
        }
    }

    private async Task AddService()
    {
        NewMonitoredService.CreatedAt = DateTime.UtcNow;
        NewMonitoredService.LastDowntimeAt = DateTime.UtcNow;
        
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.MonitoredServices.Add(NewMonitoredService);
        await db.SaveChangesAsync();
        
        NewMonitoredService = new();
        
        // TODO: Navigate to new service details page
    }

    private void SetNameAsRandomPhrase()
    {
        string phrase = $"{RandomWordGenerator.GenerateRandomAdjective(true)} {RandomWordGenerator.GenerateRandomNoun(true)}";
        NewMonitoredService.Name = phrase;
        StateHasChanged();
    }
}