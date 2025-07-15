using InterviewService.API.Extensions;
using InterviewService.BLL.Injections;
using InterviewService.DAL.Injections;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services
    .AddTelemetry(configuration)
    .AddSwaggerDocumentation()
    .AddInterviewManagmentDalLogic(configuration)
    .AddInterviewManagmentBusinessLogic()
    .AddRefitClients(configuration)
    .AddMassTransitWithConsumers(configuration)
    .AddJwtAuthentication(configuration)
    .AddCustomControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "InterviewService API v1");
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
