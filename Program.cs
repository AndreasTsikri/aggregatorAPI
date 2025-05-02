using System.Reflection;
using AggregatorAPI.Services;
using AggregatorAPI.Services.Interfaces;
using AggregatorAPI.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ExternalApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddHttpClient<IBoredApiService  , BoredApiService>();
builder.Services.AddHttpClient<IPokemonApiService, PokemonApiService>();
builder.Services.AddHttpClient<INewsApiService   , NewsApiService>();

builder.Services.AddControllers();

//Swagger for documentation/testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
