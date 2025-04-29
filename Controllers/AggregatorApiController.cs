using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AggregatorAPI.Services.Interfaces;
using AggregatorAPI.Services;

namespace AggregatorAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AggregatorApiController : ControllerBase
{
    private readonly ILogger<AggregatorApiController> _logger;
    private readonly IExternalApiService<string> _boredApiService;

    public AggregatorApiController(ILogger<AggregatorApiController> logger, IExternalApiService<string> boredApiService)
    {
        _logger = logger;
        _boredApiService = boredApiService;
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
}
