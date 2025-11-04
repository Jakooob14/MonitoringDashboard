using System.Diagnostics;
using MonitoringDashboard.Data.Models;

namespace MonitoringDashboard.Services;

public class ServiceChecker
{
    private readonly HttpClient _httpClient;

    public ServiceChecker(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _httpClient.Timeout = TimeSpan.FromSeconds(10);
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MonitoringDashboard/1.0");
    }
    
    public async Task<ServiceCheck> CheckAsync(MonitoredService service)
    {
        var result = new ServiceCheck
        {
            CheckedAt = DateTime.UtcNow
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, service.Url);
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            stopwatch.Stop();

            result.StatusCode = (int)response.StatusCode;
            result.IsSuccessful = response.IsSuccessStatusCode;
            result.ResponseTimeMilliseconds = (int)stopwatch.ElapsedMilliseconds;
        }
        catch (TaskCanceledException ex)
        {
            // Timeout or canceled request
            stopwatch.Stop();
            result.IsSuccessful = false;
            result.ResponseTimeMilliseconds = (int)stopwatch.ElapsedMilliseconds;
            result.ResponseContentSnippet = ex.InnerException?.Message ?? "Request timed out.";
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            result.IsSuccessful = false;
            result.ResponseTimeMilliseconds = (int)stopwatch.ElapsedMilliseconds;
            result.ResponseContentSnippet = ex.Message;

            if (ex.StatusCode.HasValue) result.StatusCode = (int)ex.StatusCode.Value;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.IsSuccessful = false;
            result.ResponseTimeMilliseconds = (int)stopwatch.ElapsedMilliseconds;
            result.ResponseContentSnippet = ex.Message;
        }
        
        return result;
    }
}