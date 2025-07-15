using DAL_Core;
using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CandidateService.DAL.Repositories;
public class ApplicationRepository : Repository<Application>, IApplicationRepository
{
    private readonly InterviewingSystemDbContext _context;
    public ApplicationRepository(InterviewingSystemDbContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<Application?> GetWithCandidateProfileAsync(Guid id)
    {
        return await _context.Applications
            .Include(a => a.CandidateProfile)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Application>> GetAllWithCandidateProfileAsync()
    {
        return await _context.Applications
            .Include(a => a.CandidateProfile)
            .ToListAsync();
    }

    public async Task<List<Application>> GetApplicationsByPositionId(Guid positionId)
    {
        return await _context.Applications
            .Where(a => a.PositionId == positionId)
            .ToListAsync();
    }
}
