using InterviewService.BLL.Models;

namespace InterviewService.BLL.Services.Interfaces;

public interface IAssessmentService
{
    Task<Guid> CreateAssessmentAsync(CreateAssessmentDto assessmentDto);
    Task<IEnumerable<AssessmentDto>> GetAllAssessmentsAsync();
    Task<AssessmentDto> GetAssessmentByIdAsync(Guid id);
}