using BFF.API.Models;

namespace BFF.API.Services.Interfaces;

public interface ICandidateProfileService
{
    Task<Guid> CreateCandidateProfileAsync(CreateCandidateProfileDto model);
    Task DeleteCandidateProfileAsync(Guid id);
    Task<List<CandidateProfileDto>?> GetAllCandidateProfilesAsync();
    Task<CandidateProfileDto?> GetCandidateProfileByIdAsync(Guid userId);
    Task<CandidateProfileDto?> GetCandidateProfileByUserIdAsync(Guid userId);
    Task<Guid> UpdateCandidateProfileAsync(CreateCandidateProfileDto model);
}