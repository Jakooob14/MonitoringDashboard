using Microsoft.AspNetCore.Components;
using MonitoringDashboard.Data;
using MonitoringDashboard.Helpers;

namespace MonitoringDashboard.Components.Shared.MonitoredService;

public partial class MonitoredServiceForm : ComponentBase
{
    [SupplyParameterFromForm] private Data.Models.MonitoredService NewMonitoredService { get; set; } = new();
    [Parameter] public bool EditMode { get; set; }
    [Parameter] public Data.Models.MonitoredService? EditMonitoredService { get; set; }
    

    protected override async Task OnParametersSetAsync()
    {
        if (EditMode && EditMonitoredService != null)
        {
            NewMonitoredService = EditMonitoredService;
        }
        else
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