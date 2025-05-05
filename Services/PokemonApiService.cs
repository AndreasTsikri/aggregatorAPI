using AggregatorAPI.Models;
using AggregatorAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using AggregatorAPI.Settings;
using System.Text.Json;
using System.Data;
using System.Diagnostics;

namespace AggregatorAPI.Services;

public interface IPokemonApiService : IExternalApiService<string>{

}

/// <summary>
/// 
/// </summary>
public class PokemonApiService : IPokemonApiService {
    readonly HttpClient _httpClient;
    readonly ILogger<PokemonApiService> _logger;
    readonly ExternalApiConfig _apiConfig;
    readonly IStatsApiService _statsApiService;
    readonly int MaxRetryAttempts = 3;
    readonly int DelayBetweenRetriesMilliseconds = 300;

    readonly string _endpoint  = "pokemon/ditto";
    public PokemonApiService(HttpClient httpClient, ILogger<PokemonApiService> logger, IOptions<ExternalApiSettings> options,IStatsApiService statsApiService, string endpoint = "pokemon/ditto"){
        _httpClient = httpClient;
        _logger     = logger;
        _apiConfig  = options.Value.PokemonApi;
        _endpoint   = endpoint;
        _statsApiService = statsApiService;
    }

    void UpdateStats(long? respTime = null){
        if (respTime.HasValue)
            _statsApiService.IncrementRespTime(this.GetType().Name, respTime.Value);
        else
            _statsApiService.IncrementRequestCount(this.GetType().Name);
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

                UpdateStats();
                attempt++;
                
                _logger.LogInformation("Attempt {Attempt} to call External API at {Url}", attempt, _apiConfig.BaseUrl);

                var watcher = Stopwatch.StartNew();
                var response = await _httpClient.GetAsync(_endpoint);
                watcher.Stop();
                UpdateStats(watcher.ElapsedMilliseconds);

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