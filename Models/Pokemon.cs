using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

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