using CandidateService.DAL.Repositories.Interfaces;
using DAL_Core;
using DAL_Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CandidateService.DAL.Repositories;

public class CachedPositionRepository : Repository<CachedPosition>, ICachedPositionRepository
{
    private readonly InterviewingSystemDbContext _context;
    public CachedPositionRepository(InterviewingSystemDbContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<ICollection<CachedPosition>> GetPositionsByIdsAsync(IEnumerable<Guid> positionIds)
    {
        var positions = await _context.CachedPositions
            .Where(cp => positionIds.Contains(cp.Id))
            .ToListAsync();
        return positions;
    }

    public async Task<Guid> UpsertCacheAsync(CachedPosition entity)
    {
        var existing = await _context.Set<CachedPosition>().FindAsync(entity.Id);

        if (existing != null)
        {
            _context.Entry(existing).CurrentValues.SetValues(entity);
        }
        else
        {
            await _context.Set<CachedPosition>().AddAsync(entity);
        }

        return entity.Id;
    }

}

