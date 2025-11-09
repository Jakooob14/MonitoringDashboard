using Microsoft.AspNetCore.Components;
using MonitoringDashboard.Data;
using MonitoringDashboard.Helpers;

namespace MonitoringDashboard.Components.Shared;

public partial class MonitoredServiceForm : ComponentBase
{
    [SupplyParameterFromForm] private Data.Models.MonitoredService NewMonitoredService { get; set; } = new();
    [Parameter] public bool EditMode { get; set; }
    [Parameter] public Guid ServiceId { get; set; }
    

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (EditMode)
            {
                using var scope = ScopeFactory.CreateScope();
                await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var existingService = await db.MonitoredServices.FindAsync(ServiceId);
                if (existingService == null) return;
                
                NewMonitoredService = existingService;
                StateHasChanged();
            }
            else
            {
                SetNameAsRandomPhrase();
            }
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
        Nav.NavigateTo("/dashboard/services");
    }

    private async Task UpdateService()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.MonitoredServices.Update(NewMonitoredService);
        await db.SaveChangesAsync();
        
        Nav.NavigateTo("/dashboard/services");
    }

    private void SetNameAsRandomPhrase()
    {
        string phrase = $"{RandomWordGenerator.GenerateRandomAdjective(true)} {RandomWordGenerator.GenerateRandomNoun(true)}";
        NewMonitoredService.Name = phrase;
        StateHasChanged();
    }
}