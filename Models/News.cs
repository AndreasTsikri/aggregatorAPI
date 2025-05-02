using System.Text.Json.Serialization;

namespace AggregatorAPI.Models;

public class Source{
    [JsonPropertyName("id")]
    public required string? Id{get;set;}
    [JsonPropertyName("name")]
    public required string? Name{get;set;}
}
public class Article{
    [JsonPropertyName("source")]
    public required Source Source{get;set;}
    [JsonPropertyName("author")]
    public required string Author{get;set;}
    [JsonPropertyName("title")]
    public required string Title{get;set;}
    [JsonPropertyName("content")]
    public required string Content{get;set;}
}
public class News{
    [JsonPropertyName("status")]
    public required string Status{get;set;}
    
    [JsonPropertyName("totalResults")]
    public int totalResults{get;set;}

    [JsonPropertyName("articles")]
    public required Article[] Articles{get;set;}
}