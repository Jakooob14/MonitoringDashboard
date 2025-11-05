using Microsoft.AspNetCore.Components;

namespace MonitoringDashboard.Components;

public partial class MonitoredServiceCard : ComponentBase
{
    [Parameter] public required Data.Models.MonitoredService MonitoredService { get; set; }

    private bool _wasLastCheckSuccessful;
    private List<DayStatus> _dayStatuses;

    protected override void OnParametersSet()
    {
        var lastCheck = MonitoredService.Checks.LastOrDefault();
        if (lastCheck == null) return;
        _wasLastCheckSuccessful = lastCheck.IsSuccessful;

        for (int i = 0; i < 3; i++)
        {
            var utcNow = DateTime.UtcNow;
            var startOfDay = utcNow.Date.AddDays(-i);
            var endOfDay = startOfDay.Date.AddDays(1);
            
            var todaysChecks = MonitoredService.Checks
                .Where(c => c.CheckedAt >= startOfDay && c.CheckedAt <= endOfDay)
                .ToList();

            if (!todaysChecks.Any())
            {
                _dayStatuses.Add(DayStatus.Empty);
                continue;
            }
            
            int failedCount = todaysChecks.Count(c => c.IsSuccessful == false);

            if (failedCount == 0)
            {
                _dayStatuses.Add(DayStatus.Working);
                continue;
            }
            
            if (failedCount > 0)
            {
                _dayStatuses.Add(DayStatus.Partially);
                continue;
            }
            
            if (failedCount == todaysChecks.Count)
            {
                _dayStatuses.Add(DayStatus.Failed);
                continue;
            }
        }
    }
}

enum DayStatus
{
    Working,
    Partially,
    Failed,
    Empty
}