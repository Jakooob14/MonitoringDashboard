using Microsoft.AspNetCore.Components;
using MonitoringDashboard.Data;

namespace MonitoringDashboard.Components.Shared.Maintenance;

public partial class MaintenanceCardBase : ComponentBase
{
    [Parameter] public required Data.Models.Maintenance Maintenance { get; set; }
    [Inject] public IServiceScopeFactory ScopeFactory { get; set; } = null!;
    [Inject] public NavigationManager Nav { get; set; } = null!;

    protected string GetDurationString(DateTime start, DateTime end)
    {
        TimeSpan duration = end - start;
        List<string> parts = new();

        if (duration.TotalDays >= 1)
            parts.Add($"{(int)duration.TotalDays}d");
        if (duration.Hours > 0)
            parts.Add($"{duration.Hours}h");
        if (duration.Minutes > 0)
            parts.Add($"{duration.Minutes}m");

        return string.Join(" ", parts);
    }

    protected async Task DeleteMaintenance()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entity = await db.Maintenances.FindAsync(Maintenance.Id);
        if (entity == null) return;

        db.Maintenances.Remove(entity);
        await db.SaveChangesAsync();

        Nav.NavigateTo("/dashboard/maintenance", forceLoad: true);
    }
}