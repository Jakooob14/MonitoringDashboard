using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonitoringDashboard.Data.Models;

public class Incident
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    
    [ForeignKey(nameof(MonitoredService))]
    public Guid MonitoredServiceId { get; set; }
    public MonitoredService MonitoredService { get; set; } = null!;

    [Required] public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    [MaxLength(2048)] public string? Cause { get; set; }
    [MaxLength(2048)] public string? Request { get; set; }
    [MaxLength(2048)] public string? Response { get; set; }

    [Required] public IncidentStatus Status { get; set; } = IncidentStatus.Active;
}

public enum IncidentStatus
{
    Active,
    Resolved
}