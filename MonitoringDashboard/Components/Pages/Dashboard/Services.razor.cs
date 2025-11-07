using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Pages.Dashboard;

public partial class Services
{
    private List<MonitoredService> _monitoredServices = new();
    
    protected override async Task OnInitializedAsync()
    {
        _monitoredServices = await Db.MonitoredServices
            .OrderByDescending(m => m.Name)
            .ToListAsync();
    }
}