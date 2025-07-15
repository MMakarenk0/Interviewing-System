using InterviewService.DAL.Repositories.Interfaces;

namespace InterviewService.DAL.UoF;
public interface IUnitOfWork
{
    IInterviewRepository InterviewRepository { get; }
    IInterviewSlotRepository InterviewSlotRepository { get; }
    IPositionRepository PositionRepository { get; }
    IAssessmentRepository AssessmentRepository { get; }
    ICachedApplicationRepository CachedApplicationRepository { get; set; }

    Task<int> SaveChangesAsync();
}