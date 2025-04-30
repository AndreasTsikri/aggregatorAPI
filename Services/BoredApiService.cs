using AggregatorAPI.Models;
using AggregatorAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using AggregatorAPI.Settings;
using System.Text.Json;

namespace AggregatorAPI.Services;

/// <summary>
/// This is the first External Api(Pokemon Api) that is called as a service
/// </summary>
public class BoredApiService : IExternalApiService<string> {
    readonly HttpClient _httpClient;
    readonly ILogger<BoredApiService> _logger;
    readonly  ExternalApiConfig _apiConfig;
    readonly int MaxRetryAttempts = 3;
    readonly int DelayBetweenRetriesMilliseconds = 300;

    public BoredApiService(HttpClient httpClient, ILogger<BoredApiService> logger, IOptions<ExternalApiSettings> options ){
        _httpClient = httpClient;
        _logger     = logger;
        _apiConfig  = options.Value.BoredApi;
    }

    public async Task<Result<string>> GetDataAsync(){
        
        _httpClient.BaseAddress = new Uri(_apiConfig.BaseUrl);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiConfig.ApiKey}");

        int attempt = 0;

        while (attempt < MaxRetryAttempts)
        {
            try
            {
                attempt++;

                _logger.LogInformation("Attempt {Attempt} to call External API at {Url}", attempt, _apiConfig.BaseUrl);

                //var response = await _httpClient.GetAsync("endpoint");
                var response = await _httpClient.GetAsync("pokemon/ditto");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    //return Result<string>.Success(content);
                    var pokemonResp = JsonSerializer.Deserialize<Pokemon>(content);
                    return Result<string>.Success(JsonSerializer.Serialize(pokemonResp));
                }
                else
                {
                    // Handle client errors specifically
                    if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                    {
                        _logger.LogWarning("Client error occurred: {StatusCode}", response.StatusCode);
                        return Result<string>.Failure($"Client Error: {response.StatusCode}", (int)response.StatusCode);
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