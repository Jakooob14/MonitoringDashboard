using System.ComponentModel.DataAnnotations;

namespace MonitoringDashboard.Data.Models;

public class DailyServiceStats
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [Required] public Guid MonitoredServiceId { get; set; }
    [Required] public DateTime Date { get; set; }
    [Required] public int TotalChecks { get; set; }
    [Required] public int SuccessfulChecks { get; set; }
    [Required] public int FailedChecks { get; set; }

    public double UptimePercentage => TotalChecks == 0 ? 0 : SuccessfulChecks * 100.0 / TotalChecks;
}