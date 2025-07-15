using CandidateService.BLL.Models;

namespace CandidateService.BLL.Services.Interfaces;
public interface ICandidateProfileService
{
    Task<Guid> CreateCandidateProfileAsync(CreateCandidateProfileDto candidateProfileModel);
    Task DeleteCandidateProfileAsync(Guid id);
    Task<IList<CandidateProfileDto>> FilterCandidateProfilesAsync(CandidateProfileQuery query);
    Task<IList<CandidateProfileDto>> GetAllCandidateProfilesAsync();
    Task<CandidateProfileDto> GetCandidateProfileByIdAsync(Guid id);
    Task<CandidateProfileDto?> GetCandidateProfileByUserIdAsync(Guid userId);
    Task<IList<CandidateProfileDto>> GetCandidateProfilesByUserIds(IEnumerable<Guid> userIds);
    Task<bool> IsProfileOwnedByUserAsync(Guid profileId, Guid userId);
    Task<Guid> UpdateCandidateProfileAsync(CreateCandidateProfileDto candidateProfileModel);
}