using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MonitoringDashboard.Data;
using MonitoringDashboard.Data.Models;
using MonitoringDashboard.Helpers;

namespace MonitoringDashboard.Components.Shared.Maintenance;

public partial class MaintenanceForm : ComponentBase
{
    [SupplyParameterFromForm] private Data.Models.Maintenance NewMaintenance { get; set; } = new();
    [Parameter] public bool EditMode { get; set; }
    [Parameter] public Data.Models.Maintenance? EditMaintenance { get; set; }

    [Inject] public IServiceScopeFactory ScopeFactory { get; set; } = null!;
    [Inject] public NavigationManager Nav { get; set; } = null!;

    protected List<Data.Models.MonitoredService> AllMonitoredServices { get; set; } = new();
    protected List<Guid> SelectedServiceIds { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        AllMonitoredServices = await db.MonitoredServices.ToListAsync();

        if (EditMode)
        {
            if (EditMaintenance == null) return;
            
            NewMaintenance = EditMaintenance;
            SelectedServiceIds = EditMaintenance.MonitoredServices.Select(s => s.Id).ToList();
        }
        else
        {
            NewMaintenance.StartTime = DateTime.UtcNow;
            NewMaintenance.EndTime = DateTime.UtcNow.AddHours(1);
            SetNameAsRandomPhrase();
        }
    }

    private async Task AddMaintenance()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        NewMaintenance.StartTime = ToUtc(NewMaintenance.StartTime);
        NewMaintenance.EndTime   = ToUtc(NewMaintenance.EndTime);

        var selectedServices = await db.MonitoredServices
            .Where(s => SelectedServiceIds.Contains(s.Id))
            .ToListAsync();
        NewMaintenance.MonitoredServices = selectedServices;

        db.Maintenances.Add(NewMaintenance);
        await db.SaveChangesAsync();
        Nav.NavigateTo("/dashboard/maintenance");
    }

    private async Task UpdateMaintenance()
    {
        using var scope = ScopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var existing = await db.Maintenances
            .Include(m => m.MonitoredServices)
            .FirstOrDefaultAsync(m => m.Id == NewMaintenance.Id);

        if (existing is null) return;

        existing.Title = NewMaintenance.Title;
        existing.Description = NewMaintenance.Description;
        existing.Severity = NewMaintenance.Severity;
        existing.Status = NewMaintenance.Status;

        existing.StartTime = ToUtc(NewMaintenance.StartTime);
        existing.EndTime = ToUtc(NewMaintenance.EndTime);

        var selectedServices = await db.MonitoredServices
            .Where(s => SelectedServiceIds.Contains(s.Id))
            .ToListAsync();

        existing.MonitoredServices = selectedServices;

        await db.SaveChangesAsync();
        Nav.NavigateTo("/dashboard/maintenance");
    }
    
    private static DateTime ToUtc(DateTime dt) => dt.Kind switch
    {
        DateTimeKind.Utc => dt,
        DateTimeKind.Local => dt.ToUniversalTime(),
        DateTimeKind.Unspecified => DateTime.SpecifyKind(dt, DateTimeKind.Local).ToUniversalTime()
    };

    private void SetNameAsRandomPhrase()
    {
        string phrase = $"{RandomWordGenerator.GenerateRandomAdjective(true)} {RandomWordGenerator.GenerateRandomNoun(true)}";
        NewMaintenance.Title = phrase;
        StateHasChanged();
    }
    
    private void ToggleService(Guid id)
    {
        if (SelectedServiceIds.Contains(id))
            SelectedServiceIds.Remove(id);
        else
            SelectedServiceIds.Add(id);
    }
    
    private string StartLocalString
    {
        get => NewMaintenance.StartTime.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");
        set
        {
            if (DateTime.TryParse(value, out var local))
                NewMaintenance.StartTime = DateTime.SpecifyKind(local, DateTimeKind.Local).ToUniversalTime();
        }
    }

    private string EndLocalString
    {
        get => NewMaintenance.EndTime.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");
        set
        {
            if (DateTime.TryParse(value, out var local))
                NewMaintenance.EndTime = DateTime.SpecifyKind(local, DateTimeKind.Local).ToUniversalTime();
        }
    }
}
