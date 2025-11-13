using System.ComponentModel.DataAnnotations;

namespace MonitoringDashboard.Data.Models;

public class User
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [Required, MaxLength(100)] public string Username { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Email { get; set; } = string.Empty;
    [Required] public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}