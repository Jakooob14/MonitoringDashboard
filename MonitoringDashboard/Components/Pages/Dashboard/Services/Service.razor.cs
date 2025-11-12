using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages.Dashboard.Services;

public partial class Service : ComponentBase
{
    [Parameter] public Guid Id { get; set; }
    
    private List<Incident> _serviceIncidents = new();
    private int _maxIncidentsToShow = 5;
    private int _totalIncidentsCount;

    protected override async Task OnParametersSetAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        _totalIncidentsCount = await db.Incidents
            .Where(i => i.MonitoredServiceId == Id)
            .CountAsync();
        
        await UpdateIncidents(1);
    }
    
    private async Task UpdateIncidents(int currentPage)
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        _serviceIncidents = await db.Incidents
            .Include(i => i.MonitoredService)
            .Where(i => i.MonitoredServiceId == Id)
            .OrderByDescending(i => i.StartTime)
            .Skip((currentPage - 1) * _maxIncidentsToShow)
            .Take(_maxIncidentsToShow)
            .ToListAsync();

        await InvokeAsync(StateHasChanged);
    }
    
    private async Task HandleMaxIncidentsChanged(int maxIncidents)
    {
        Console.WriteLine(maxIncidents);
        _maxIncidentsToShow = maxIncidents;
        await UpdateIncidents(1);
    }
}