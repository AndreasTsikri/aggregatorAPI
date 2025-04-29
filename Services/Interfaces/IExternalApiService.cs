using AggregatorAPI.Models;

namespace AggregatorAPI.Services.Interfaces;
public interface IExternalApiService<T>{
    Task<Result<T>> GetDataAsync();
}