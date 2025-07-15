using BFF.API.Models;
using Refit;

namespace BFF.API.Clients;

public interface ICandidateServiceApi
{
    #region CandidateProfile
    [Get("/api/CandidateProfile/{id}")]
    Task<CandidateProfileDto> GetCandidateProfileByIdAsync(Guid id);

    [Get("/api/CandidateProfile")]
    Task<List<CandidateProfileDto>> GetAllCandidateProfilesAsync();

    [Post("/api/CandidateProfile/by-ids")]
    Task<List<CandidateProfileDto>> GetCandidateProfilesByIdsAsync([Body] IdListRequest request);

    [Post("/api/CandidateProfile/filter")]
    Task<List<CandidateProfileDto>> FilterCandidateProfiles([Body] CandidateProfileQuery query);

    [Post("/api/CandidateProfile")]
    Task<Guid> CreateCandidateProfileAsync([Body] CreateCandidateProfileDto model);

    [Delete("/api/CandidateProfile/{id}")]
    Task DeleteCandidateProfileAsync(Guid id);

    [Multipart]
    [Put("/api/CandidateProfile")]
    Task<Guid> UpdateCandidateProfileAsync(
        [AliasAs("Id")] string id,
        [AliasAs("UserId")] string userId,
        [AliasAs("ProfileResumeFile")] StreamPart? profileResumeFile,
        [AliasAs("YearsOfExperience")] int? yearsOfExperience,
        [AliasAs("CurrentPosition")] string? currentPosition,
        [AliasAs("TechStack")] string? techStack,
        [AliasAs("ApplicationIds")] IEnumerable<string>? applicationIds
    );

    [Get("/api/CandidateProfile/by-user/{userId}")]
    Task<CandidateProfileDto?> GetCandidateProfileByUserIdAsync(Guid userId);
    #endregion

    #region Application

    [Get("/api/Application/{id}")]
    Task<ApplicationDto> GetApplicationByIdAsync(Guid id);

    [Get("/api/Application")]
    Task<List<ApplicationDto>> GetAllApplicationsAsync();

    [Get("/api/Application/positions/{positionId}/applications")]
    Task<List<ApplicationDto>> GetApplicationsByPositionIdAsync(Guid positionId);

    [Multipart]
    [Post("/api/Application")]
    Task<Guid> CreateApplicationAsync(
        [AliasAs("Id")] string Id,
        [AliasAs("CandidateProfileId")] string candidateProfileId,
        [AliasAs("PositionId")] string positionId,
        [AliasAs("ResumeFile")] StreamPart resumeFile
    );

    [Multipart]
    [Put("/api/Application")]
    Task<Guid> UpdateApplicationAsync(
        [AliasAs("Id")] string id,
        [AliasAs("CandidateProfileId")] string candidateProfileId,
        [AliasAs("PositionId")] string positionId,
        [AliasAs("ResumeFile")] StreamPart? resumeFile
    );

    [Delete("/api/Application/{id}")]
    Task DeleteApplicationAsync(Guid id);

    [Post("/api/Application/approve/{id}")]
    Task ApproveApplicationAsync(Guid id);

    #endregion

}
