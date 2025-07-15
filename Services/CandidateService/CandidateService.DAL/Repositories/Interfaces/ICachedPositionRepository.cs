using DAL_Core.Entities;

namespace CandidateService.DAL.Repositories.Interfaces;

public interface ICachedPositionRepository : IRepository<CachedPosition>
{
    Task<ICollection<CachedPosition>> GetPositionsByIdsAsync(IEnumerable<Guid> positionIds);
    Task<Guid> UpsertCacheAsync(CachedPosition entity);
}