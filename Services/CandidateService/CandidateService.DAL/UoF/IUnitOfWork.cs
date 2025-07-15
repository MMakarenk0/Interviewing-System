using CandidateService.DAL.Repositories;
using CandidateService.DAL.Repositories.Interfaces;

namespace CandidateService.DAL.UoF;
public interface IUnitOfWork
{
    ICandidateProfileRepository CandidateProfileRepository { get; }
    IApplicationRepository ApplicationRepository { get; }
    ICachedPositionRepository CachedPositionRepository { get; set; }

    Task<int> SaveChangesAsync();
}