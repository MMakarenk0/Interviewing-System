using DAL_Core;
using InterviewService.DAL.Repositories.Interfaces;

namespace InterviewService.DAL.UoF;
public class UnitOfWork : IUnitOfWork
{
    private readonly InterviewingSystemDbContext _context;

    public IInterviewRepository InterviewRepository { get; }
    public IInterviewSlotRepository InterviewSlotRepository { get; }
    public IPositionRepository PositionRepository { get; }
    public IAssessmentRepository AssessmentRepository { get; }
    public ICachedApplicationRepository CachedApplicationRepository { get; set; }

    public UnitOfWork(InterviewingSystemDbContext context,
        IInterviewRepository interviewRepository,
        IInterviewSlotRepository interviewSlotRepository,
        IPositionRepository positionRepository,
        IAssessmentRepository assessmentRepository,
        ICachedApplicationRepository cachedApplicationRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        InterviewRepository = interviewRepository;
        InterviewSlotRepository = interviewSlotRepository;
        PositionRepository = positionRepository;
        AssessmentRepository = assessmentRepository;
        CachedApplicationRepository = cachedApplicationRepository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

