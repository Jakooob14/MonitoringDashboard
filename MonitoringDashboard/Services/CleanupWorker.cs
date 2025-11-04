using Microsoft.EntityFrameworkCore;

namespace MonitoringDashboard.Services;

public class CleanupWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<CleanupWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("CleanupWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                await using var db = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();

                var cutoffDate = DateTime.UtcNow.AddDays(-30);
                logger.LogInformation("Starting cleanup for entries before {Cutoff}", cutoffDate);

                const int batchSize = 5000;
                int totalDeleted = 0;

                while (true)
                {
                    var oldChecks = await db.ServiceChecks
                        .Where(c => c.CheckedAt < cutoffDate)
                        .OrderBy(c => c.CheckedAt)
                        .Take(batchSize)
                        .ToListAsync(stoppingToken);

                    if (oldChecks.Count == 0) break;

                    db.ServiceChecks.RemoveRange(oldChecks);
                    await db.SaveChangesAsync(stoppingToken);
                    totalDeleted += oldChecks.Count;

                    logger.LogInformation("Deleted {Count} old records...", oldChecks.Count);
                }

                logger.LogInformation("Cleanup complete. Total deleted: {Total}", totalDeleted);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occurred while cleaning up old data.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }

        logger.LogInformation("CleanupWorker stopping.");
    }
}
