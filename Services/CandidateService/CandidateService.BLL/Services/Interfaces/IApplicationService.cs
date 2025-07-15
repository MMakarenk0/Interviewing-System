using CandidateService.BLL.Enum;
using CandidateService.BLL.Models;

namespace CandidateService.BLL.Services.Interfaces;
public interface IApplicationService
{
    Task<Guid> CreateApplicationAsync(CreateApplicationDto model);
    Task DeleteApplicationAsync(Guid id);
    Task<IList<ApplicationDto>> GetAllApplicationsAsync();
    Task<ApplicationDto> GetApplicationByIdAsync(Guid id);
    Task<Guid> UpdateApplicationAsync(CreateApplicationDto model);
    Task<bool> IsApplicationOwnedByUserAsync(Guid applicationId, Guid userId);
    Task ApproveApplicationAsync(Guid applicationId);
    Task ScheduleInterviewAsync(Guid applicationId, Guid interviewId);
    Task FinalizeApplicationAsync(Guid applicationId, ApplicationFinalStatus finalStatus);
    Task<IList<ApplicationDto>> GetApplicationsByPositionIdAsync(Guid positionId);
}