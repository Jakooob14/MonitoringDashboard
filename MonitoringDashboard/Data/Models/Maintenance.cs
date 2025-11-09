using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonitoringDashboard.Data.Models;

public class Maintenance
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [Required, MaxLength(128)] public string Title { get; set; } = string.Empty;
    [MaxLength(2048)] public string Description { get; set; } = string.Empty;
    [Required] public DateTime StartTime { get; set; }
    [Required] public DateTime EndTime { get; set; }
    [Required] public bool IsCompleted { get; set; }

    public ICollection<MonitoredService> MonitoredServices { get; set; } = new List<MonitoredService>();
}