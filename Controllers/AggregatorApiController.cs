using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AggregatorAPI.Services.Interfaces;
using AggregatorAPI.Services;
using AggregatorAPI.Models;

namespace AggregatorAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AggregatorApiController : ControllerBase
{
    private readonly ILogger<AggregatorApiController> _logger;
    private readonly IExternalApiService<string> _boredApiService;
    private readonly IExternalApiService<string> _pokemonApiService;

    public AggregatorApiController(ILogger<AggregatorApiController> logger, IBoredApiService boredApiService, IPokemonApiService pokemonApiService)
    {
        _logger            = logger;
        _boredApiService   = boredApiService;
        _pokemonApiService = pokemonApiService;
    }


    /// <summary>
    /// The endpoint to get all aggregated data from all APIs. 
    /// Now is only calling the 1 API
    /// </summary>
    /// <returns> An asynchronous IActionResult using the Ok or StatusCode</returns>
    [HttpGet("externaldata")]
    public async Task<IActionResult> GetExternalData()
    {
        var result = await _boredApiService.GetDataAsync();

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        else
        {
            _logger.LogWarning("Returning error to client: {ErrorMessage}", result.ErrorMessage);
            return StatusCode(result.StatusCode, result.ErrorMessage);
        }
    }

    [HttpGet("externaldatatwo")]
    public async Task<IActionResult> GetDataTwoApis()
    {
        var api1 = _boredApiService.GetDataAsync();
        var api2 = _pokemonApiService.GetDataAsync();

        await Task.WhenAll(api1,api2);

        Result<string> r1 = await api1;
        Result<string> r2 = await api2;

        if (!r1.IsSuccess)
        {
             _logger.LogWarning("First api error to client: {ErrorMessage}", r1.ErrorMessage);
        }
        if (!r2.IsSuccess)
        {
             _logger.LogWarning("Second api error to client: {ErrorMessage}", r2.ErrorMessage);
        }        
        return Ok(r1.Data + "\n" + r2.Data);
    }
}
