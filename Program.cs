using AggregatorAPI.Services;
using AggregatorAPI.Services.Interfaces;
using AggregatorAPI.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ExternalApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddHttpClient<IExternalApiService<string>, BoredApiService>();

builder.Services.AddControllers();

//Swagger for documentation/testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
