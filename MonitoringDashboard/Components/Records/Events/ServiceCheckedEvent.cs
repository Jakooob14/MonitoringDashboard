namespace MonitoringDashboard.Components.Records.Events;

public record ServiceCheckedEvent(
    Guid ServiceId,
    bool IsSuccessful,
    DateTime CheckedAt
);