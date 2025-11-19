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

                // === Service checks ===
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

                // === Maintenance status updates ===
                var now = DateTime.UtcNow;
                var maintenances = await db.Maintenances.ToListAsync(stoppingToken);
                
                foreach (var maintenance in maintenances)
                {
                    if (maintenance.Status == MaintenanceStatus.Scheduled &&
                        maintenance.StartTime <= now)
                    {
                        maintenance.Status = MaintenanceStatus.InProgress;
                        db.Maintenances.Update(maintenance);
                        logger.LogInformation("Maintenance {MaintenanceId} started.", maintenance.Id);
                    }
                    else if (maintenance.Status == MaintenanceStatus.InProgress &&
                             maintenance.EndTime <= now)
                    {
                        maintenance.Status = MaintenanceStatus.Completed;
                        db.Maintenances.Update(maintenance);
                        logger.LogInformation("Maintenance {MaintenanceId} completed.", maintenance.Id);
                    }
                }
                
                // === Incident management ===
                foreach (var service in services)
                {
                    var lastCheck = await db.ServiceChecks
                        .Where(c => c.MonitoredServiceId == service.Id)
                        .OrderByDescending(c => c.CheckedAt)
                        .FirstOrDefaultAsync(stoppingToken);

                    if (lastCheck != null &&
                        (DateTime.UtcNow - lastCheck.CheckedAt).TotalSeconds < service.CheckIntervalSeconds)
                        continue;

                    var check = await serviceChecker.CheckAsync(service);
                    check.MonitoredServiceId = service.Id;
                    db.ServiceChecks.Add(check);

                    // --- Incident logic ---
                    var activeIncident = await db.Set<Incident>()
                        .Where(i => i.MonitoredServiceId == service.Id && i.Status == IncidentStatus.Active)
                        .FirstOrDefaultAsync(stoppingToken);

                    if (!check.IsSuccessful)
                    {
                        service.LastDowntimeAt = DateTime.UtcNow;
                        db.MonitoredServices.Update(service);

                        if (activeIncident == null)
                        {
                            // Create new incident
                            var incident = new Incident
                            {
                                MonitoredServiceId = service.Id,
                                StartTime = DateTime.UtcNow,
                                Status = IncidentStatus.Active
                            };
                            db.Add(incident);
                            logger.LogWarning("Incident created for service {ServiceName}", service.Name);
                        }
                    }
                    else
                    {
                        // Service is healthy again
                        if (activeIncident != null)
                        {
                            activeIncident.Status = IncidentStatus.Resolved;
                            activeIncident.EndTime = DateTime.UtcNow;
                            db.Update(activeIncident);
                            logger.LogInformation("Incident resolved for service {ServiceName}", service.Name);
                        }
                    }
                }

                // === Save everything ===
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
