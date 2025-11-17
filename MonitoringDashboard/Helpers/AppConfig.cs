namespace MonitoringDashboard.Helpers;

public static class AppConfig
{
    private static string GetConfigValue(string key)
    {
        return Environment.GetEnvironmentVariable(key) 
               ?? throw new InvalidOperationException($"Missing env var: {key}");
    }
    
    public static readonly string AppName = GetConfigValue("APP_NAME") ?? "Monitoring Dashboard";
}