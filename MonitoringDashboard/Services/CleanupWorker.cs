using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data.Models;

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

                // Aggregation
                var yesterday = DateTime.UtcNow.Date.AddDays(-1);

                logger.LogInformation("Starting daily rollup for {Date}", yesterday);

                var alreadyRolled = await db.DailyServiceStats
                    .AnyAsync(d => d.Date == yesterday, stoppingToken);
                if (!alreadyRolled)
                {
                    var dailyStats = await db.ServiceChecks
                        .Where(c => c.CheckedAt.Date == yesterday)
                        .GroupBy(c => c.MonitoredServiceId)
                        .Select(g => new DailyServiceStats
                        {
                            MonitoredServiceId = g.Key,
                            Date = yesterday,
                            TotalChecks = g.Count(),
                            SuccessfulChecks = g.Count(x => x.IsSuccessful),
                            FailedChecks = g.Count(x => !x.IsSuccessful)
                        })
                        .ToListAsync(stoppingToken);

                    if (dailyStats.Count > 0)
                    {
                        db.DailyServiceStats.AddRange(dailyStats);
                        await db.SaveChangesAsync(stoppingToken);
                        logger.LogInformation("Inserted {Count} daily rollup records for {Date}.", dailyStats.Count, yesterday);
                    }
                    else
                    {
                        logger.LogInformation("No data found for {Date}, skipping rollup.", yesterday);
                    }
                }
                else
                {
                    logger.LogInformation("Rollup for {Date} already exists, skipping.", yesterday);
                }

                // Cleanup
                var cutoffDate = DateTime.UtcNow.AddDays(-3);
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

                    if (oldChecks.Count == 0)
                        break;

                    db.ServiceChecks.RemoveRange(oldChecks);
                    await db.SaveChangesAsync(stoppingToken);
                    totalDeleted += oldChecks.Count;

                    logger.LogInformation("Deleted {Count} old records...", oldChecks.Count);
                }

                logger.LogInformation("Cleanup complete. Total deleted: {Total}", totalDeleted);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occurred during cleanup/rollup.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }

        logger.LogInformation("CleanupWorker stopping.");
    }
}
