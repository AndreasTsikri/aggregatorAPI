using System.Collections.Concurrent;

namespace AggregatorAPI.Services;

public interface IStatsApiService
{
    void IncrementRequestCount(string apiName);
    void IncrementRespTime(string apiName, long respTime);
    int GetReqCount(string apiName);
    long GetRespTime(string apiName);
    //Dictionary<string, int> GetAllStats();
}


public class StatisticsApiService : IStatsApiService{
    public readonly ConcurrentDictionary<string, int> _reqCount = new ConcurrentDictionary<string, int>();
    public readonly ConcurrentDictionary<string, long> _respTime = new ConcurrentDictionary<string, long>();
    readonly ILogger<IStatsApiService> _logger;
    public StatisticsApiService(ILogger<IStatsApiService> l)
    {
        _logger  = l;
    }

    public void IncrementRequestCount(string apiName){
        if (string.IsNullOrWhiteSpace(apiName))
            return;        
        _reqCount.AddOrUpdate(apiName, 1, (k, v) => v + 1);        
    }
    public void IncrementRespTime(string apiName, long timeInMilli){
        if (string.IsNullOrWhiteSpace(apiName))
            return;
        _respTime.AddOrUpdate(apiName, timeInMilli, (k,v) => v + timeInMilli);
    }
    
    public int GetReqCount(string apiName){
        return _reqCount.TryGetValue(apiName, out int v) ? v :0;
    }
    public long GetRespTime(string apiName){
        return _respTime.TryGetValue(apiName, out long v) ? v :0;
    }
}