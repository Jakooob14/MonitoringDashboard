using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Components.Shared.Enums;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages.Dashboard.Services;

public partial class Service
{
    [Parameter] public Guid Id { get; set; }

    private List<Incident> _serviceIncidents = new();
    private List<ServiceCheck> _recentChecks = new();
    private int _maxIncidentsToShow = 5;
    private int _totalIncidentsCount;

    private float _uptimeDayPercentage = 0f;

    protected override async Task OnParametersSetAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        _totalIncidentsCount = await db.Incidents
            .Where(i => i.MonitoredServiceId == Id)
            .CountAsync();
        
        MonitoredService = (await db.MonitoredServices
            .FirstOrDefaultAsync(ms => ms.Id == Id))!;
        
        _recentChecks = await db.ServiceChecks
            .Where(sc => sc.MonitoredServiceId == Id)
            .OrderByDescending(sc => sc.CheckedAt)
            .ToListAsync();
        
        await UpdateIncidents(1);
        
        await base.OnParametersSetAsync();
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
    
    private int GetAverageResponseTime(TimeSpan timeSpan)
    {
        if (_recentChecks.Count == 0) return 0;

        var cutoff = DateTime.UtcNow - timeSpan;
        var checks = _recentChecks.Where(c => c.CheckedAt >= cutoff).ToList();
        if (!checks.Any()) return 0;

        return (int)checks.Average(c => c.ResponseTimeMilliseconds);
    }
    
    private async Task HandleMaxIncidentsChanged(int maxIncidents)
    {
        _maxIncidentsToShow = maxIncidents;
        await UpdateIncidents(1);
    }
}