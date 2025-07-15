using InterviewService.BLL.Models;

namespace InterviewService.BLL.Services.Interfaces;

public interface IInterviewService
{
    Task<Guid> CreateInterviewAsync(CreateInterviewDto model);
    Task DeleteInterviewAsync(Guid id);
    Task<IList<InterviewDto>> GetAllInterviewsAsync();
    Task<InterviewDto> GetInterviewByIdAsync(Guid id);
    Task<bool> IsInterviewOwnedByUserAsync(Guid interviewId, Guid userId);
    Task<Guid> UpdateInterviewAsync(CreateInterviewDto model);
}