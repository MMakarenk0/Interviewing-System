using CandidateService.DAL.Repositories;
using CandidateService.DAL.Repositories.Interfaces;
using DAL_Core;

namespace CandidateService.DAL.UoF;
public class UnitOfWork : IUnitOfWork
{
    private readonly InterviewingSystemDbContext _context;

    public ICandidateProfileRepository CandidateProfileRepository { get; }
    public IApplicationRepository ApplicationRepository { get; }
    public ICachedPositionRepository CachedPositionRepository { get; set; }

    public UnitOfWork(InterviewingSystemDbContext context,
        ICandidateProfileRepository candidateProfileRepository,
        IApplicationRepository applicationRepository,
        ICachedPositionRepository cachedPositionRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        CandidateProfileRepository = candidateProfileRepository;
        ApplicationRepository = applicationRepository;
        CachedPositionRepository = cachedPositionRepository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

