using DAL_Core;
using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CandidateService.DAL.Repositories;
public class CandidateProfileRepository : Repository<CandidateProfile>, ICandidateProfileRepository
{
    private readonly InterviewingSystemDbContext _context;
    public CandidateProfileRepository(InterviewingSystemDbContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<CandidateProfile?> GetCandidateProfileWithApplicationsAsync(Guid id)
    {
        return await _context.CandidateProfiles
            .Include(x => x.Applications)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<CandidateProfile?> GetCandidateProfileByUserIdAsync(Guid userId)
    {
        return await _context.CandidateProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<ICollection<CandidateProfile>> GetCandidateProfilesByUserIds(IEnumerable<Guid> userIds)
    {
        return await _context.CandidateProfiles
            .Where(x => userIds.Contains(x.UserId))
            .ToListAsync();
    }

    public IQueryable<CandidateProfile> Query()
    {
        return _context.CandidateProfiles.AsQueryable();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Set<CandidateProfile>()
            .AnyAsync(cp => cp.Id == id);
    }

    public async Task<string?> GetProfileResumePathAsync(Guid candidateProfileId)
    {
        return await _context.Set<CandidateProfile>()
            .Where(cp => cp.Id == candidateProfileId)
            .Select(cp => cp.ProfileResumeBlobPath)
            .FirstOrDefaultAsync();
    }
}

