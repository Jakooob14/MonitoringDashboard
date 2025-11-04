using System.ComponentModel.DataAnnotations;

namespace MonitoringDashboard.Data.Models;

public class MonitoredService
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [Required, MaxLength(128)] public string Name { get; set; } = string.Empty;
    [MaxLength(1024)] public string Url { get; set; } = string.Empty;
    [Range(5, int.MaxValue)] public int CheckIntervalSeconds { get; set; } = 60;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<ServiceCheck> Checks { get; set; } = new List<ServiceCheck>();
}