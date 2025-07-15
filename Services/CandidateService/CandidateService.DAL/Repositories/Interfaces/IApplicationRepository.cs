using CandidateService.DAL.Repositories.Interfaces;
using DAL_Core.Entities;

namespace CandidateService.DAL.Repositories;
public interface IApplicationRepository : IRepository<Application>
{
    Task<List<Application>> GetAllWithCandidateProfileAsync();
    Task<List<Application>> GetApplicationsByPositionId(Guid positionId);
    Task<Application?> GetWithCandidateProfileAsync(Guid id);
}