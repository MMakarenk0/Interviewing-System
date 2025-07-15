using CandidateService.API.Extensions;
using CandidateService.BLL.Injections;
using CandidateService.DAL.Injections;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Service Registration
builder.Services
    .AddTelemetry(configuration)
    .AddSwaggerDocumentation()
    .AddCandidateManagmentDalLogic(configuration)
    .AddCandidateManagmentBusinessLogic()
    .AddRefitClients(configuration)
    .AddMassTransitWithConsumers(configuration)
    .AddJwtAuthentication(configuration)
    .AddCustomControllers();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CandidateService API v1");
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
