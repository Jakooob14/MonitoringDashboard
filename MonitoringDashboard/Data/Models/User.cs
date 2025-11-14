using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MonitoringDashboard.Data.Models;

public class User : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}