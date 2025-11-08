using Microsoft.EntityFrameworkCore;

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

                var services = await db.MonitoredServices.ToListAsync(stoppingToken);

                foreach (var service in services)
                {
                    var lastCheck = await db.ServiceChecks
                        .Where(c => c.MonitoredServiceId == service.Id)
                        .OrderByDescending(c => c.CheckedAt)
                        .FirstOrDefaultAsync(stoppingToken);
                    
                    // Skip if checked recently
                    if (lastCheck != null && (DateTime.UtcNow - lastCheck.CheckedAt).TotalSeconds <
                        service.CheckIntervalSeconds) continue;
                    
                    var check = await serviceChecker.CheckAsync(service);

                    check.MonitoredServiceId = service.Id;

                    db.ServiceChecks.Add(check);
                    
                    if (!check.IsSuccessful)
                    {
                        service.LastDowntimeAt = DateTime.UtcNow;
                        db.MonitoredServices.Update(service);
                    }
                }
                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occurred while checking services.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        logger.LogInformation("MonitoringWorker stopping.");
    }
}