using AggregatorAPI.Models;

namespace AggregatorAPI.Services.Interfaces;
public interface IExternalApiService<T>{
    Task<Result<T>> GetDataAsync();
}
public interface IExternalApiServiceWithParams<T>{
    Task<Result<T>> GetDataAsync(string q, string sortBy);
}