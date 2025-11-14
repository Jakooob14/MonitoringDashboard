using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<MonitoredService> MonitoredServices => Set<MonitoredService>();
    public DbSet<ServiceCheck> ServiceChecks => Set<ServiceCheck>();
    public DbSet<DailyServiceStats> DailyServiceStats => Set<DailyServiceStats>();
    public DbSet<Maintenance> Maintenances => Set<Maintenance>();
    public DbSet<Incident> Incidents => Set<Incident>();
}
