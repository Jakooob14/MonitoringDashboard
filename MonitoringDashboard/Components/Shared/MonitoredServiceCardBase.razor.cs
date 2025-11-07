using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Components.Shared.Enums;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Shared;

public partial class MonitoredServiceCardBase : ComponentBase
{
    [Parameter] public required MonitoredService MonitoredService { get; set; }
    [Inject] public IServiceScopeFactory ScopeFactory { get; set; } = null!;

    protected List<Status> DayStatuses { get; private set; } = new();
    private List<ServiceCheck> _recentChecks = new();
    
    protected bool IsServiceUp()
    {
        var lastCheck = _recentChecks.FirstOrDefault();
        return lastCheck is { IsSuccessful: true };
    }
    
    protected float GetUptimePercentage()
    {
        if (_recentChecks.Count == 0)
        {
            return 0;
        }
        
        int successfulChecks = _recentChecks.Count(c => c.IsSuccessful);
        float percentage = (float)successfulChecks / _recentChecks.Count * 100;
        
        return (float)Math.Round(percentage, 3);
    }
    
    protected string GetLastIncidentString()
    {
        var lastFailedCheck = _recentChecks
            .Where(c => !c.IsSuccessful)
            .OrderByDescending(c => c.CheckedAt)
            .FirstOrDefault();

        if (lastFailedCheck == null)
        {
            return "No incidents recently";
        }

        var timeSpan = DateTime.UtcNow - lastFailedCheck.CheckedAt;
        
        if (timeSpan.TotalDays >= 1)
        {
            return $"Last incident {(int)timeSpan.TotalDays} day{(timeSpan.TotalDays > 1 ? "s" : "")} ago";
        }
        if (timeSpan.TotalHours >= 1)
        {
            return $"Last incident {(int)timeSpan.TotalHours} hour{(timeSpan.TotalHours > 1 ? "s" : "")} ago";
        }
        if (timeSpan.TotalMinutes >= 1)
        {
            return $"Last incident {(int)timeSpan.TotalMinutes} minute{(timeSpan.TotalMinutes > 1 ? "s" : "")} ago";
        }
        
        return "Last incident just now";
    }
    
    protected override async Task OnParametersSetAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        _recentChecks = await db.ServiceChecks
            .Where(c => c.MonitoredServiceId == MonitoredService.Id && c.CheckedAt >= DateTime.UtcNow.AddDays(-30))
            .OrderByDescending(c => c.CheckedAt)
            .ToListAsync();
        
        DayStatuses.Clear();
        
        UpdateRecentChecks();
    }
    
    protected string GetFormattedUptime()
    {
        var lastDowntime = MonitoredService.LastDowntimeAt;
        var now = DateTime.UtcNow;

        var span = now - lastDowntime;

        List<string> parts = new();

        int years = span.Days / 365;
        int months = span.Days % 365 / 30;
        int days = span.Days % 365 % 30;
        int hours = span.Hours;
        int minutes = span.Minutes;

        if (years > 0) parts.Add($"{years} year{(years > 1 ? "s" : "")}");
        if (months > 0) parts.Add($"{months} month{(months > 1 ? "s" : "")}");
        if (days > 0) parts.Add($"{days} day{(days > 1 ? "s" : "")}");
        if (hours > 0) parts.Add($"{hours} hour{(hours > 1 ? "s" : "")}");
        if (minutes > 0) parts.Add($"{minutes} minute{(minutes > 1 ? "s" : "")}");

        string formatted = parts.Count > 0
            ? "Up " + string.Join(", ", parts.Take(2))
            : "Down";

        return formatted;
    }
    
    private void UpdateRecentChecks()
    {
        for (int i = 0; i < 90; i++)
        {
            var utcNow = DateTime.UtcNow;
            var startOfDay = utcNow.Date.AddDays(-i);
            var endOfDay = startOfDay.Date.AddDays(1);
            
            var todaysChecks = _recentChecks
                .Where(c => c.CheckedAt >= startOfDay && c.CheckedAt < endOfDay)
                .ToList();

            if (!todaysChecks.Any())
            {
                DayStatuses.Add(Status.Empty);
                continue;
            }
            
            int successfulChecks = todaysChecks.Count(c => c.IsSuccessful);

            if (successfulChecks == todaysChecks.Count)
            {
                DayStatuses.Add(Status.Working);
            } else if ((float)successfulChecks / todaysChecks.Count > 0.75f)
            {
                DayStatuses.Add(Status.Degraded);
            } else
            {
                DayStatuses.Add(Status.Failed);
            }
        }
    }
}