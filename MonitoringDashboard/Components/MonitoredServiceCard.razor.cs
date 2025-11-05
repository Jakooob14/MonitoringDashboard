using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components;

public partial class MonitoredServiceCard : ComponentBase
{
    [Parameter] public required MonitoredService MonitoredService { get; set; }

    private bool _wasLastCheckSuccessful;
    private List<DayStatus> _dayStatuses = new();
    private List<ServiceCheck> _recentChecks = new();
    private float _uptimePercentage;

    protected override async Task OnParametersSetAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        _recentChecks = await db.ServiceChecks
            .Where(c => c.MonitoredServiceId == MonitoredService.Id && c.CheckedAt >= DateTime.UtcNow.AddDays(-30))
            .OrderByDescending(c => c.CheckedAt)
            .ToListAsync();
        
        _dayStatuses.Clear();
        
        var lastCheck = _recentChecks.LastOrDefault();
        if (lastCheck == null) return;
        _wasLastCheckSuccessful = lastCheck.IsSuccessful;
        
        UpdateRecentChecks();
        CalculateUptimePercentage();
    }
    
    private void UpdateRecentChecks()
    {
        for (int i = 0; i < 30; i++)
        {
            var utcNow = DateTime.UtcNow;
            var startOfDay = utcNow.Date.AddDays(-i);
            var endOfDay = startOfDay.Date.AddDays(1);
            
            var todaysChecks = _recentChecks
                .Where(c => c.CheckedAt >= startOfDay && c.CheckedAt < endOfDay)
                .ToList();

            if (!todaysChecks.Any())
            {
                _dayStatuses.Add(DayStatus.Empty);
                continue;
            }
            
            int successfulChecks = todaysChecks.Count(c => c.IsSuccessful);

            if (successfulChecks == todaysChecks.Count)
            {
                _dayStatuses.Add(DayStatus.Working);
            } else if ((float)successfulChecks / todaysChecks.Count > 0.75f)
            {
                _dayStatuses.Add(DayStatus.Partially);
            } else
            {
                _dayStatuses.Add(DayStatus.Failed);
            }
        }
    }
    
    private void CalculateUptimePercentage()
    {
        if (_recentChecks.Count == 0)
        {
            _uptimePercentage = 0;
            return;
        }
        
        int successfulChecks = _recentChecks.Count(c => c.IsSuccessful);
        float percentage = (float)successfulChecks / _recentChecks.Count * 100;
        
        _uptimePercentage = (float)Math.Round(percentage, 3);
    }
}

enum DayStatus
{
    Working,
    Partially,
    Failed,
    Empty
}