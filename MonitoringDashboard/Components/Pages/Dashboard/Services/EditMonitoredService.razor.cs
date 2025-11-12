using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data;

namespace MonitoringDashboard.Components.Pages.Dashboard.Services;

public partial class EditMonitoredService
{
    [Parameter] public Guid Id { get; set; }
    private Data.Models.MonitoredService? MonitoredService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var monitoredService = await db.MonitoredServices
            .FirstOrDefaultAsync(m => m.Id == Id);

        if (monitoredService == null)
        {
            Nav.NavigateTo("/404");
            return;
        }

        MonitoredService = monitoredService;
    }
}