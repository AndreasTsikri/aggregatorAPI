using System.Text.Json.Serialization;

namespace AggregatorAPI.Models;

public class Bored{
    [JsonPropertyName("activity")]
    public required string Activity{get;set;}
    
    [JsonPropertyName("type")]
    public string? Type{get;set;}

    [JsonPropertyName("participants")]
    public int Participants{get;set;}
}