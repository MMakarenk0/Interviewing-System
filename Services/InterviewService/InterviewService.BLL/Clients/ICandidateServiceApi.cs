using InterviewService.BLL.Models;
using Refit;

namespace InterviewService.BLL.Clients;

public interface ICandidateServiceApi
{
    [Get("/api/application/{id}")]
    Task<ApplicationDto?> GetApplicationByIdAsync(Guid id);
}

