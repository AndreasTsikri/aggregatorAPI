
namespace AggregatorAPI.Models;
public class Result<T>{

    public bool IsSuccess{get; set;}
    public int StatusCode {get;set;}
    public string? ErrorMessage {get;set;}
    public T? Data {get;set;}

    public static Result<T> Success(T data) => new Result<T>{IsSuccess = true, StatusCode = 200, Data = data};
    public static Result<T> Failure(string err, int status = 500) => new Result<T>{IsSuccess = false, StatusCode = status, ErrorMessage = err,};

}