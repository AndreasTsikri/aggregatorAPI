namespace AggregatorAPI.Settings;

public class ExternalApiSettings
{
    public required ExternalApiConfig BoredApi { get; set; }
    public required ExternalApiConfig PokemonApi { get; set; }
    public required ExternalApiConfig NewsApi { get; set; }
}

public class ExternalApiConfig
{
    public required string BaseUrl { get ; set; }
    public string? ApiKey { get; set; }
}
