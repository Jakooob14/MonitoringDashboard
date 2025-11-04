using Microsoft.EntityFrameworkCore;

namespace MonitoringDashboard.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // public DbSet<a> a => Set<a>();
}