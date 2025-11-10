using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Services;

public class MonitoringWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<MonitoringWorker> logger,
    ServiceChecker serviceChecker)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("MonitoringWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                await using var db = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();

                // === 1. Service checks ===
                var services = await db.MonitoredServices.AsNoTracking().ToListAsync(stoppingToken);

                foreach (var service in services)
                {
                    var lastCheck = await db.ServiceChecks
                        .Where(c => c.MonitoredServiceId == service.Id)
                        .OrderByDescending(c => c.CheckedAt)
                        .FirstOrDefaultAsync(stoppingToken);

                    // Skip if checked recently
                    if (lastCheck != null &&
                        (DateTime.UtcNow - lastCheck.CheckedAt).TotalSeconds < service.CheckIntervalSeconds)
                        continue;

                    var check = await serviceChecker.CheckAsync(service);
                    check.MonitoredServiceId = service.Id;

                    db.ServiceChecks.Add(check);

                    if (!check.IsSuccessful)
                    {
                        service.LastDowntimeAt = DateTime.UtcNow;
                        db.MonitoredServices.Update(service);
                    }
                }

                // === 2. Maintenance status updates ===
                var now = DateTime.UtcNow;
                var maintenances = await db.Maintenances.ToListAsync(stoppingToken);

                foreach (var m in maintenances)
                {
                    // Active window to In Progress
                    if (m.StartTime <= now && now < m.EndTime && m.Status != MaintenanceStatus.Completed)
                    {
                        if (m.Status != MaintenanceStatus.InProgress)
                        {
                            m.Status = MaintenanceStatus.InProgress;
                            logger.LogInformation("Maintenance '{Title}' is now IN PROGRESS", m.Title);
                            db.Maintenances.Update(m);
                        }
                    }
                    // After window to Completed
                    else if (m.EndTime <= now && m.Status != MaintenanceStatus.Completed)
                    {
                        m.Status = MaintenanceStatus.Completed;
                        logger.LogInformation("Maintenance '{Title}' marked as COMPLETED", m.Title);
                        db.Maintenances.Update(m);
                    }
                    // Before start to Scheduled
                    else if (now < m.StartTime && m.Status == MaintenanceStatus.InProgress)
                    {
                        m.Status = MaintenanceStatus.Scheduled;
                        db.Maintenances.Update(m);
                    }
                }

                // === 3. Save everything ===
                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occurred while checking services or updating maintenance.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        logger.LogInformation("MonitoringWorker stopping.");
    }
}
