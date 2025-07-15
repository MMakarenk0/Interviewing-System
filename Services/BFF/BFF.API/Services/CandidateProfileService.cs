using BFF.API.Clients;
using BFF.API.Models;
using BFF.API.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Refit;

namespace BFF.API.Services;

public class CandidateProfileService : ICandidateProfileService
{
    private readonly ICandidateServiceApi _candidateProfileApi;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

    public CandidateProfileService(ICandidateServiceApi candidateProfileApi, IMemoryCache cache)
    {
        _candidateProfileApi = candidateProfileApi;
        _cache = cache;
    }

    public async Task<CandidateProfileDto?> GetCandidateProfileByIdAsync(Guid userId)
    {
        var cacheKey = GetProfileByIdCacheKey(userId);

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await _candidateProfileApi.GetCandidateProfileByIdAsync(userId);
        });
    }

    public async Task<CandidateProfileDto?> GetCandidateProfileByUserIdAsync(Guid userId)
    {
        var cacheKey = GetProfileByUserIdCacheKey(userId);

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await _candidateProfileApi.GetCandidateProfileByUserIdAsync(userId);
        });
    }

    public async Task<List<CandidateProfileDto>?> GetAllCandidateProfilesAsync()
    {
        // TODO: Add pagination support .
        var cacheKey = "candidate:all";
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            var allProfiles = await _candidateProfileApi.GetAllCandidateProfilesAsync();
            return allProfiles;
        });
    }

    public async Task<Guid> CreateCandidateProfileAsync(CreateCandidateProfileDto model)
    {
        var id = await _candidateProfileApi.CreateCandidateProfileAsync(model);

        await InvalidateCacheAsync(id, model.UserId);
        return id;
    }

    public async Task DeleteCandidateProfileAsync(Guid id)
    {
        await _candidateProfileApi.DeleteCandidateProfileAsync(id);
        _cache.Remove(GetProfileByIdCacheKey(id));
    }

    public async Task<Guid> UpdateCandidateProfileAsync(CreateCandidateProfileDto model)
    {
        StreamPart? filePart = null;
        if (model.ProfileResumeFile != null)
        {
            var stream = model.ProfileResumeFile.OpenReadStream();
            filePart = new StreamPart(stream, model.ProfileResumeFile.FileName, model.ProfileResumeFile.ContentType);
        }

        IEnumerable<string>? appIds = model.ApplicationIds?.Select(g => g.ToString());

        var id = await _candidateProfileApi.UpdateCandidateProfileAsync(
            model.Id.ToString(),
            model.UserId.ToString(),
            filePart,
            model.YearsOfExperience,
            model.CurrentPosition,
            model.TechStack,
            appIds
        );

        await InvalidateCacheAsync(model.Id, model.UserId);
        return id;
    }

    private static string GetProfileByIdCacheKey(Guid id) => $"candidate:id:{id}";
    private static string GetProfileByUserIdCacheKey(Guid userId) => $"candidate:user:{userId}";

    private Task InvalidateCacheAsync(Guid candidateId, Guid userId)
    {
        _cache.Remove(GetProfileByIdCacheKey(candidateId));
        _cache.Remove(GetProfileByUserIdCacheKey(userId));
        return Task.CompletedTask;
    }
}
