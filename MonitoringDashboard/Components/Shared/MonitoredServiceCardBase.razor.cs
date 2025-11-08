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

    protected override void OnInitialized()
    {
        ServiceUp = false;
        Statuses = Enumerable.Repeat(Status.Empty, RecentUnitsToShow).ToList();
        LastIncidentString = "Loading...";
        UptimePercentage = 0f;
    }

    private bool IsServiceUp()
    {
        var lastCheck = _recentChecks.FirstOrDefault();
        return lastCheck is { IsSuccessful: true };
    }

    private float GetUptimePercentage()
    {
        if (_recentChecks.Count == 0)
            return 0;

        int successfulChecks = _recentChecks.Count(c => c.IsSuccessful);
        return (float)Math.Round((float)successfulChecks / _recentChecks.Count * 100, 3);
    }

    private string GetLastIncidentString()
    {
        var lastFailedCheck = _recentChecks
            .Where(c => !c.IsSuccessful)
            .OrderByDescending(c => c.CheckedAt)
            .FirstOrDefault();

        if (lastFailedCheck == null) return "No incidents recently";

        var timeSpan = DateTime.UtcNow - lastFailedCheck.CheckedAt;

        if (timeSpan.TotalDays >= 1) return $"Last incident {(int)timeSpan.TotalDays} day{(timeSpan.TotalDays > 1 ? "s" : "")} ago";
        if (timeSpan.TotalHours >= 1) return $"Last incident {(int)timeSpan.TotalHours} hour{(timeSpan.TotalHours > 1 ? "s" : "")} ago";
        if (timeSpan.TotalMinutes >= 1) return $"Last incident {(int)timeSpan.TotalMinutes} minute{(timeSpan.TotalMinutes > 1 ? "s" : "")} ago";

        return "Last incident just now";
    }

    protected override async Task OnParametersSetAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        DateTime cutoff = DisplayUnit switch
        {
            TimeUnit.Days => DateTime.UtcNow.AddDays(-RecentUnitsToShow),
            TimeUnit.Hours => DateTime.UtcNow.AddHours(-RecentUnitsToShow),
            _ => DateTime.UtcNow.AddDays(-RecentUnitsToShow)
        };

        _recentChecks = await db.ServiceChecks
            .Where(c => c.MonitoredServiceId == MonitoredService.Id && c.CheckedAt >= cutoff)
            .OrderByDescending(c => c.CheckedAt)
            .ToListAsync();

        UpdateStatuses();

        ServiceUp = IsServiceUp();
        LastIncidentString = GetLastIncidentString();
        UptimePercentage = GetUptimePercentage();

        StateHasChanged();
    }

    private void UpdateStatuses()
    {
        Statuses.Clear();

        if (DisplayUnit == TimeUnit.Days)
            UpdateDailyStatuses();
        else
            UpdateHourlyStatuses();
    }

    private void UpdateDailyStatuses()
    {
        var utcNow = DateTime.UtcNow.Date;

        for (int i = 0; i < RecentUnitsToShow; i++)
        {
            var start = utcNow.AddDays(-i);
            var end = start.AddDays(1);

            AddStatusRange(start, end);
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

        int successCount = checks.Count(c => c.IsSuccessful);
        float ratio = (float)successCount / checks.Count;

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
}

public enum TimeUnit
{
    Days,
    Hours
}
