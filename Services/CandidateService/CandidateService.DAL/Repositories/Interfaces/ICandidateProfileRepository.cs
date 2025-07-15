using CandidateService.DAL.Repositories.Interfaces;
using DAL_Core.Entities;

namespace CandidateService.DAL.Repositories;
public interface ICandidateProfileRepository : IRepository<CandidateProfile>
{
    Task<bool> ExistsAsync(Guid id);
    Task<CandidateProfile?> GetCandidateProfileByUserIdAsync(Guid id);
    Task<ICollection<CandidateProfile>> GetCandidateProfilesByUserIds(IEnumerable<Guid> userIds);
    Task<CandidateProfile?> GetCandidateProfileWithApplicationsAsync(Guid id);
    Task<string?> GetProfileResumePathAsync(Guid candidateProfileId);
    IQueryable<CandidateProfile> Query();
}