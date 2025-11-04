using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonitoringDashboard.Data.Models;

public class ServiceCheck
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [ForeignKey(nameof(MonitoredService))] public Guid MonitoredServiceId { get; set; }
    public MonitoredService MonitoredService { get; set; } = null!;
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public bool IsSuccessful { get; set; }
    public int ResponseTimeMilliseconds { get; set; }
    public int StatusCode { get; set; }
    [MaxLength(2048)] public string? ResponseContentSnippet { get; set; }
}