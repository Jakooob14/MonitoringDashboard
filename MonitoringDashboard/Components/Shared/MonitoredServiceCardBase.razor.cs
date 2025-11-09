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

    [Parameter] public TimeUnit DisplayUnit { get; set; } = TimeUnit.Days;
    [Parameter] public int RecentUnitsToShow { get; set; } = 90;

    protected List<Status> Statuses { get; private set; } = new();
    protected bool ServiceUp;
    protected string LastIncidentString = string.Empty;
    protected float UptimePercentage;

    private List<ServiceCheck> _recentChecks = new();
    private List<DailyServiceStats> _recentStats = new();

    protected override void OnInitialized()
    {
        ServiceUp = false;
        Statuses = Enumerable.Repeat(Status.Empty, RecentUnitsToShow).ToList();
        LastIncidentString = "Loading...";
        UptimePercentage = 0f;
    }

    protected override async Task OnParametersSetAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        DateTime cutoff = DisplayUnit switch
        {
            TimeUnit.Days => DateTime.UtcNow.Date.AddDays(-RecentUnitsToShow),
            TimeUnit.Hours => DateTime.UtcNow.AddHours(-RecentUnitsToShow),
            _ => DateTime.UtcNow.Date.AddDays(-RecentUnitsToShow)
        };

        if (DisplayUnit == TimeUnit.Days)
        {
            // Load historical rollups
            _recentStats = await db.DailyServiceStats
                .Where(s => s.MonitoredServiceId == MonitoredService.Id && s.Date >= cutoff)
                .OrderByDescending(s => s.Date)
                .ToListAsync();

            // Also load today's raw checks and merge them into a synthetic "today" stat
            var today = DateTime.UtcNow.Date;
            var todayChecks = await db.ServiceChecks
                .Where(c => c.MonitoredServiceId == MonitoredService.Id && c.CheckedAt >= today)
                .ToListAsync();

            if (todayChecks.Any())
            {
                var todayStat = new DailyServiceStats
                {
                    MonitoredServiceId = MonitoredService.Id,
                    Date = today,
                    TotalChecks = todayChecks.Count,
                    SuccessfulChecks = todayChecks.Count(c => c.IsSuccessful),
                    FailedChecks = todayChecks.Count(c => !c.IsSuccessful)
                };

                // Replace or insert today's stat
                _recentStats.RemoveAll(s => s.Date.Date == today);
                _recentStats.Insert(0, todayStat);
            }
        }
        else
        {
            // Hourly view = raw only
            _recentChecks = await db.ServiceChecks
                .Where(c => c.MonitoredServiceId == MonitoredService.Id && c.CheckedAt >= cutoff)
                .OrderByDescending(c => c.CheckedAt)
                .ToListAsync();
        }

        UpdateStatuses();

        ServiceUp = await IsServiceUp(db);
        LastIncidentString = await GetLastIncidentString(db);
        UptimePercentage = GetUptimePercentage();

        StateHasChanged();
    }

    private async Task<bool> IsServiceUp(AppDbContext db)
    {
        // Always prefer live data if it's recent (within 1 day)
        var lastCheck = await db.ServiceChecks
            .Where(c => c.MonitoredServiceId == MonitoredService.Id)
            .OrderByDescending(c => c.CheckedAt)
            .FirstOrDefaultAsync();

        if (lastCheck != null && (DateTime.UtcNow - lastCheck.CheckedAt).TotalHours < 24)
            return lastCheck.IsSuccessful;

        // Fallback: use the latest daily rollup if no recent raw checks exist
        var latestRollup = await db.DailyServiceStats
            .Where(s => s.MonitoredServiceId == MonitoredService.Id)
            .OrderByDescending(s => s.Date)
            .FirstOrDefaultAsync();

        if (latestRollup != null)
            return latestRollup.FailedChecks < latestRollup.TotalChecks;

        // Default if no data
        return false;
    }

    private float GetUptimePercentage()
    {
        if (DisplayUnit == TimeUnit.Days)
        {
            if (_recentStats.Count == 0)
                return 0;

            int total = _recentStats.Sum(s => s.TotalChecks);
            int ok = _recentStats.Sum(s => s.SuccessfulChecks);
            return total == 0 ? 0 : (float)Math.Round(ok * 100f / total, 2);
        }

        if (_recentChecks.Count == 0)
            return 0;

        int successful = _recentChecks.Count(c => c.IsSuccessful);
        return (float)Math.Round(successful * 100f / _recentChecks.Count, 2);
    }

    private async Task<string> GetLastIncidentString(AppDbContext db)
    {
        if (DisplayUnit == TimeUnit.Days)
        {
            // Try today's raw failed checks first
            var lastFailedRaw = await db.ServiceChecks
                .Where(c => c.MonitoredServiceId == MonitoredService.Id && !c.IsSuccessful)
                .OrderByDescending(c => c.CheckedAt)
                .FirstOrDefaultAsync();

            if (lastFailedRaw != null)
            {
                var timeSpan = DateTime.UtcNow - lastFailedRaw.CheckedAt;

                if (timeSpan.TotalDays >= 1) return $"Last incident {(int)timeSpan.TotalDays} day{(timeSpan.TotalDays >= 2 ? "s" : "")} ago";
                if (timeSpan.TotalHours >= 1) return $"Last incident {(int)timeSpan.TotalHours} hour{(timeSpan.TotalHours >= 2 ? "s" : "")} ago";
                if (timeSpan.TotalMinutes >= 1) return $"Last incident {(int)timeSpan.TotalMinutes} minute{(timeSpan.TotalMinutes >= 2 ? "s" : "")} ago";

                return "Last incident just now";
            }

            // Fallback to historical rollup if no recent raw failures
            var lastFailedStat = await db.DailyServiceStats
                .Where(s => s.MonitoredServiceId == MonitoredService.Id && s.FailedChecks > 0)
                .OrderByDescending(s => s.Date)
                .FirstOrDefaultAsync();

            if (lastFailedStat == null)
                return "No incidents recently";

            var daysAgo = (DateTime.UtcNow.Date - lastFailedStat.Date).TotalDays;
            return daysAgo switch
            {
                >= 1 => $"Last incident {(int)daysAgo} day{(daysAgo >= 2 ? "s" : "")} ago",
                _ => "Last incident today"
            };
        }

        // Hourly view = raw only
        var lastFailedCheck = _recentChecks
            .Where(c => !c.IsSuccessful)
            .OrderByDescending(c => c.CheckedAt)
            .FirstOrDefault();

        if (lastFailedCheck == null) return "No incidents recently";

        var ts = DateTime.UtcNow - lastFailedCheck.CheckedAt;
        if (ts.TotalDays >= 1) return $"Last incident {(int)ts.TotalDays} day{(ts.TotalDays >= 2 ? "s" : "")} ago";
        if (ts.TotalHours >= 1) return $"Last incident {(int)ts.TotalHours} hour{(ts.TotalHours >= 2 ? "s" : "")} ago";
        if (ts.TotalMinutes >= 1) return $"Last incident {(int)ts.TotalMinutes} minute{(ts.TotalMinutes >= 2 ? "s" : "")} ago";

        return "Last incident just now";
    }


    private void UpdateStatuses()
    {
        Statuses.Clear();

        if (DisplayUnit == TimeUnit.Days)
            UpdateDailyStatusesFromRollup();
        else
            UpdateHourlyStatuses();
    }

    private void UpdateDailyStatusesFromRollup()
    {
        var utcNow = DateTime.UtcNow.Date;

        for (int i = 0; i < RecentUnitsToShow; i++)
        {
            var date = utcNow.AddDays(-i);
            var stat = _recentStats.FirstOrDefault(s => s.Date.Date == date);

            if (stat == null)
            {
                Statuses.Add(Status.Empty);
                continue;
            }

            float ratio = (float)stat.SuccessfulChecks / stat.TotalChecks;
            Status status = ratio switch
            {
                1f => Status.Working,
                > 0.75f => Status.Degraded,
                _ => Status.Failed
            };

            Statuses.Add(status);
        }
    }

    private void UpdateHourlyStatuses()
    {
        var utcNow = DateTime.UtcNow;

        for (int i = 0; i < RecentUnitsToShow; i++)
        {
            var start = utcNow.AddHours(-i - 1);
            var end = utcNow.AddHours(-i);
            AddStatusRange(start, end);
        }
    }

    private void AddStatusRange(DateTime start, DateTime end)
    {
        var checks = _recentChecks.Where(c => c.CheckedAt >= start && c.CheckedAt < end).ToList();

        if (!checks.Any())
        {
            Statuses.Add(Status.Empty);
            return;
        }

        int success = checks.Count(c => c.IsSuccessful);
        float ratio = (float)success / checks.Count;

        Status status = ratio switch
        {
            1f => Status.Working,
            > 0.75f => Status.Degraded,
            _ => Status.Failed
        };

        Statuses.Add(status);
    }

    protected string GetFormattedUptime()
    {
        var span = DateTime.UtcNow - MonitoredService.LastDowntimeAt;
        List<string> parts = new();

        int years = span.Days / 365;
        int months = span.Days % 365 / 30;
        int days = span.Days % 365 % 30;
        int hours = span.Hours;
        int minutes = span.Minutes;

        if (years > 0) parts.Add($"{years} year{(years >= 2 ? "s" : "")}");
        if (months > 0) parts.Add($"{months} month{(months >= 2 ? "s" : "")}");
        if (days > 0) parts.Add($"{days} day{(days >= 2 ? "s" : "")}");
        if (hours > 0) parts.Add($"{hours} hour{(hours >= 2 ? "s" : "")}");
        if (minutes > 0) parts.Add($"{minutes} minute{(minutes >= 2 ? "s" : "")}");

        return parts.Count > 0
            ? "Up " + string.Join(", ", parts.Take(2))
            : "Down";
    }
}

public enum TimeUnit
{
    Days,
    Hours
}
