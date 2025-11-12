using Microsoft.AspNetCore.Components;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Components.Shared.Incidents;

public partial class IncidentsTable
{
    [Parameter, EditorRequired] public List<Incident> ServiceIncidents { get; set; }
    [Parameter, EditorRequired] public int TotalIncidentsCount { get; set; }
    [Parameter] public int MaxIncidentsToShow { get; set; } = 5;
    [Parameter] public EventCallback<int> OnPageChanged { get; set; }
    [Parameter] public EventCallback<int> OnMaxIncidentsToShowChanged { get; set; }
    
    private int _currentPage = 1;
    private int TotalPages => (int)Math.Ceiling((double)TotalIncidentsCount / MaxIncidentsToShow);

    private async Task HandlePageChanged(int relativePage)
    {
        int newPage = _currentPage + relativePage;
        
        if (newPage > TotalPages) newPage = TotalPages;

        if (newPage < 1) newPage = 1;
        
        _currentPage = newPage;

        if (OnPageChanged.HasDelegate)
        {
            await OnPageChanged.InvokeAsync(newPage);
        }
    }
    
    private void HandleMaxIncidentsChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out int newMax) && newMax > 0)
        {
            MaxIncidentsToShow = newMax;
            _currentPage = 1;
            OnMaxIncidentsToShowChanged.InvokeAsync(newMax);
        }
    }
}