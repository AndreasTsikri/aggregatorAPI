using AggregatorAPI.Models;
using AggregatorAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using AggregatorAPI.Settings;
using System.Text.Json;

namespace AggregatorAPI.Services;


public interface INewsApiService : IExternalApiServiceWithParams<string> {

}

/// <summary>
/// This is the third External Api(News Api) service
/// </summary>
public class NewsApiService : INewsApiService{
    readonly HttpClient _httpClient;
    readonly ILogger<NewsApiService> _logger;
    readonly  ExternalApiConfig _apiConfig;
    readonly int MaxRetryAttempts = 3;
    readonly int DelayBetweenRetriesMilliseconds = 300;

    string _endpoint  = "";
    public NewsApiService(HttpClient httpClient, ILogger<NewsApiService> logger, IOptions<ExternalApiSettings> options, string endpoint = ""){
        _httpClient = httpClient;
        _logger     = logger;
        _apiConfig  = options.Value.NewsApi;
        _endpoint   = endpoint;
    }

    public async Task<Result<string>> GetDataAsync(string q, string? sortBy){
        
        _httpClient.BaseAddress = new Uri(_apiConfig.BaseUrl);
        //_httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", $"{_apiConfig.ApiKey}");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "C# app");
        
        

        _endpoint = string.IsNullOrWhiteSpace(sortBy) ? $"?q={q}" : $"?q={q}&sortBy={sortBy}";
        
        int attempt = 0;

        while (attempt < MaxRetryAttempts)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                    throw new ArgumentException("q parameter must not be empty");
         
                attempt++;

                _logger.LogInformation("Attempt {Attempt} to call External API at {Url}", attempt, _apiConfig.BaseUrl);
                
                var response = await _httpClient.GetAsync(_endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    
                    var newsResp = JsonSerializer.Deserialize<News>(content);
                    return Result<string>.Success(JsonSerializer.Serialize(newsResp));
                }
                else
                {
                                        // Handle client errors specifically
                    if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        _logger.LogWarning("Client error occurred: {StatusCode}", response.StatusCode);
                        _logger.LogWarning("Client error occurred: {Content}", content);
                        return Result<string>.Failure($"Client Error: {response.StatusCode}, ErrorMessage: {content}");
                    }
                    else
                    {
                        _logger.LogWarning("Server error or network issue: {StatusCode}", response.StatusCode);
                        // We retry only on server/network errors
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, $"HttpRequestException on attempt {attempt}");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, $"TaskCanceledException (timeout) on attempt {attempt}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected exception on attempt {attempt}");
                return Result<string>.Failure("Unexpected internal error.", 500);
            }

            if (attempt < MaxRetryAttempts)
            {
                await Task.Delay(DelayBetweenRetriesMilliseconds);
            }
        }
        _logger.LogError("All {MaxRetryAttempts} attempts to call External API have failed.", MaxRetryAttempts);
        return Result<string>.Failure("Failed to retrieve data after multiple attempts.", 503);
    }
}