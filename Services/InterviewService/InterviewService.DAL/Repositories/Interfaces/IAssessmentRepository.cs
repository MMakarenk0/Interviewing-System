using DAL_Core.Entities;

namespace InterviewService.DAL.Repositories.Interfaces;
public interface IAssessmentRepository : IRepository<Assessment>
{
    Task<Assessment?> GetWithInterviewAsync(Guid id);
}