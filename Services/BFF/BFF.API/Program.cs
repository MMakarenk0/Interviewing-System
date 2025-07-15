using BFF.API.Extensions;
using BFF.API.Injections;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services
    .AddCustomTelemetry(configuration)
    .AddCustomCors()
    .AddCustomRateLimiter();

builder.Services.AddMemoryCache();
builder.Services.AddBFFServices();
builder.Services.AddRefitClients(configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthenticationAndAuthorization(configuration);

var app = builder.Build();

app.UseCustomMiddlewares();

app.MapControllers();

app.Run();
