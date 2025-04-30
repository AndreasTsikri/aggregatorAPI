using System.Text.Json.Serialization;

namespace AggregatorAPI.Models;
public class Pokemon{

    [JsonPropertyName("name")]
    public required string Name{get;set;}

    [JsonPropertyName("height")]
    public int Height {get;set;}

    [JsonPropertyName("weight")]
    public int Weight {get;set;}

    [JsonPropertyName("location_area_encounters")]
    public string? Location_Area_Encounters {get;set;}
    
}