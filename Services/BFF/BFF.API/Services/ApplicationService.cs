using BFF.API.Clients;
using BFF.API.Models;
using BFF.API.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Refit;
using System.Net.Http.Headers;

namespace BFF.API.Services;

public class ApplicationService : IApplicationService
{
    private readonly ICandidateServiceApi _candidateServiceApi;
    private readonly IMemoryCache _cache;
    private const int CacheDurationInMinutes = 3;

    private const string AllApplicationsCacheKey = "applications:all";

    public ApplicationService(
        ICandidateServiceApi candidateServiceApi,
        IMemoryCache cache)
    {
        _candidateServiceApi = candidateServiceApi;
        _cache = cache;
    }

    public async Task<ApplicationDto> GetApplicationByIdAsync(Guid id)
    {
        string cacheKey = $"application:{id}";

        if (_cache.TryGetValue(cacheKey, out ApplicationDto cachedApp))
            return cachedApp;

        var application = await _candidateServiceApi.GetApplicationByIdAsync(id);

        _cache.Set(cacheKey, application, TimeSpan.FromMinutes(CacheDurationInMinutes));

        return application;
    }


    public async Task<List<ApplicationDto>> GetAllApplicationsAsync()
    {
        if (_cache.TryGetValue(AllApplicationsCacheKey, out List<ApplicationDto> cachedApps))
            return cachedApps;

        var applications = await _candidateServiceApi.GetAllApplicationsAsync();

        _cache.Set(AllApplicationsCacheKey, applications, TimeSpan.FromMinutes(3));

        return applications;
    }

    public async Task<List<ApplicationDto>> GetApplicationsByPositionIdAsync(Guid positionId)
    {
        string cacheKey = $"applications:byPosition:{positionId}";

        if (_cache.TryGetValue(cacheKey, out List<ApplicationDto> cachedApps))
            return cachedApps;

        var applications = await _candidateServiceApi.GetApplicationsByPositionIdAsync(positionId);

        _cache.Set(cacheKey, applications, TimeSpan.FromMinutes(CacheDurationInMinutes));

        return applications;
    }

    public async Task<Guid> CreateApplicationAsync(CreateApplicationDto dto)
    {
        StreamPart? resumePart = null;

        if (dto.ResumeFile != null)
        {
            resumePart = ConvertFormFileToStreamPart(dto.ResumeFile, "ResumeFile");
        }

        var result = await _candidateServiceApi.CreateApplicationAsync(
            dto.Id.ToString(),
            dto.CandidateProfileId.ToString(),
            dto.PositionId.ToString(),
            resumePart
        );

        InvalidateApplicationCache(dto.Id);

        return result;
    }


    public async Task<Guid> UpdateApplicationAsync(CreateApplicationDto dto)
    {
        StreamPart? resumePart = dto.ResumeFile != null
            ? ConvertFormFileToStreamPart(dto.ResumeFile, "ResumeFile")
            : null;

        var result = await _candidateServiceApi.UpdateApplicationAsync(
            dto.Id.ToString(),
            dto.CandidateProfileId.ToString(),
            dto.PositionId.ToString(),
            resumePart
        );

        InvalidateApplicationCache(dto.Id);

        return result;
    }

    public async Task DeleteApplicationAsync(Guid id)
    {
        await _candidateServiceApi.DeleteApplicationAsync(id);
        InvalidateApplicationCache(id);
    }

    public async Task ApproveApplicationAsync(Guid id)
    {
        await _candidateServiceApi.ApproveApplicationAsync(id);
        InvalidateApplicationCache(id);
    }

    private StreamPart ConvertFormFileToStreamPart(IFormFile file, string name)
    {
        var stream = file.OpenReadStream();
        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"') ?? file.FileName;
        return new StreamPart(stream, fileName, file.ContentType);
    }
    private void InvalidateApplicationCache(Guid applicationId, Guid? candidateProfileId = null, Guid? positionId = null)
    {
        _cache.Remove($"application:{applicationId}");
        _cache.Remove(AllApplicationsCacheKey);

        if (candidateProfileId.HasValue)
            _cache.Remove($"applications:byCandidateProfile:{candidateProfileId.Value}");

        if (positionId.HasValue)
            _cache.Remove($"applications:byPosition:{positionId.Value}");
    }

}
