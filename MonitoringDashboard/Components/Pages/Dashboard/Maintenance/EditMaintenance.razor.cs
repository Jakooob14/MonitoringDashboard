using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data;

namespace MonitoringDashboard.Components.Pages.Dashboard.Maintenance;

public partial class EditMaintenance
{
    [Parameter] public Guid Id { get; set; }
    private Data.Models.Maintenance? Maintenance { get; set; }

    protected override async Task OnInitializedAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var maintenance = await db.Maintenances
            .Include(m => m.MonitoredServices)
            .FirstOrDefaultAsync(m => m.Id == Id);

        if (maintenance == null)
        {
            Nav.NavigateTo("/404");
            return;
        }

        Maintenance = maintenance;
    }
}