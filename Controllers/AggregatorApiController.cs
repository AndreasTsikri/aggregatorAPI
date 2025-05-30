using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AggregatorAPI.Services.Interfaces;
using AggregatorAPI.Services;
using AggregatorAPI.Models;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace AggregatorAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AggregatorApiController : ControllerBase
{
    private readonly ILogger<AggregatorApiController> _logger;
    private readonly IExternalApiService<string> _boredApiService;
    private readonly IExternalApiService<string> _pokemonApiService;
    private readonly IExternalApiServiceWithParams<string> _newsApiService;
    private readonly IStatsApiService _statsApiService;
    
    public AggregatorApiController(ILogger<AggregatorApiController> logger, IBoredApiService boredApiService, IPokemonApiService pokemonApiService, INewsApiService newsApiService, IStatsApiService statsApiService)
    {
        _logger            = logger;
        _boredApiService   = boredApiService;
        _pokemonApiService = pokemonApiService;
        _newsApiService    = newsApiService;
        _statsApiService   = statsApiService;
    }


    
    [HttpGet("getBoredApiData")]
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

    /// <summary>
    /// The endpoint to get all aggregated data from all APIs. 
    /// Now is calling 3 APIs : pokemon API ,Bored Api and News Api. 
    /// Only news Api needs an API key to be called
    /// </summary>
    /// <returns> An asynchronous IActionResult using the Ok or StatusCode</returns>
    [HttpGet("externaldatathree")]
    public async Task<IActionResult> GetAggregatedData([FromQuery][Required] string q, [FromQuery] string? sortBy)//public async Task<IActionResult> GetDataTwoApis()
    {
        string getResponse(Result<string>? r){
            if (r == null)
                return  "";
            return r.IsSuccess ? r?.Data ?? "No data" : r?.ErrorMessage ?? "No error message";
        }

        

        var api1 = _boredApiService.GetDataAsync();
        var api2 = _pokemonApiService.GetDataAsync();
        var api3 = _newsApiService.GetDataAsync(q, sortBy);

        await Task.WhenAll(api1, api2, api3);

        Result<string> r1 = await api1;
        Result<string> r2 = await api2;
        Result<string> r3 = await api3;

        if (!r1.IsSuccess)
        {
             _logger.LogWarning("First api error to client: {ErrorMessage}", r1.ErrorMessage);             
        }
        if (!r2.IsSuccess)
        {
             _logger.LogWarning("Second api error to client: {ErrorMessage}", r2.ErrorMessage);
        }       
        if (!r3.IsSuccess)
        {
             _logger.LogWarning("Third api error to client: {ErrorMessage}", r3.ErrorMessage);
        }       
        
        // var t = new Aggr<AggregateResult>(
        //     new AggregateResult{
        //         ApiUrl = "test - bored",
        //         Result = getResponse(r1)
        //     },
        //     new AggregateResult{
        //         ApiUrl = "test - pokemon",
        //         Result = getResponse(r2)
        //     });
        return Ok(getResponse(r1) + ", " + getResponse(r2) + ", " + getResponse(r3));
    }

    [HttpGet("statistics")]
    public IActionResult GetStatistics()
    {

        try{
            
            string calculatePerformanceGroup(long avg) => avg <=0 ?"undefined" : avg < 100 ? "fast" : avg < 200 ? "medium" : "slow";
            
            string api1name = _boredApiService.GetType().Name; 
            string api2name = _pokemonApiService.GetType().Name; 
            string api3name = _newsApiService.GetType().Name; 
            var apiN = new string[]{api1name,api2name, api3name};

            var rd = new Dictionary<string, string>();
            foreach(var n in apiN){
                var count = _statsApiService.GetReqCount(n);
                var respTime = _statsApiService.GetRespTime(n);
                var avg = count != 0 ? respTime / (long) count : 0;
                rd.Add(n, $"count:{count}, total response time:{respTime}, avg response time:{avg}, performance:{calculatePerformanceGroup(avg)}"); 
            }
            return Ok(rd);
         }
        catch (Exception ex){
            _logger.LogWarning($"Problem getting statistics : {ex.Message}");
            return StatusCode(500, "Internal server error, please check logs");    
        }

    }
}