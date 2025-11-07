using Microsoft.AspNetCore.Components;

namespace MonitoringDashboard.Components.Shared;

public partial class RefreshTimer : ComponentBase, IDisposable
{
    [Parameter] public EventCallback OnRefresh { get; set; }
    
    private readonly int _timeToRefresh = 60;
    private System.Timers.Timer? _timer;
    private int _remainingTimeToRefresh;

    public void Dispose()
    {
        _timer?.Stop();
        _timer?.Dispose();
    }
    
    protected override void OnInitialized()
    {
        _remainingTimeToRefresh = _timeToRefresh;
        
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += async (_, _) =>
        {
            _remainingTimeToRefresh--;

            if (_remainingTimeToRefresh < 0)
            {
                _remainingTimeToRefresh = _timeToRefresh;
                await InvokeAsync(async () => await OnRefresh.InvokeAsync());
            }

            await InvokeAsync(StateHasChanged);
        };
        _timer.Start();
    }
}