using BFF.API.Clients;
using BFF.API.Models;
using BFF.API.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BFF.API.Services;

public class PositionService : IPositionService
{
    private readonly IInterviewServiceApi _interviewServiceApi;
    private readonly IMemoryCache _cache;
    private const int CacheDurationInMinutes = 5;

    private static int _version = 1;

    public PositionService(IInterviewServiceApi interviewServiceApi, IMemoryCache cache)
    {
        _interviewServiceApi = interviewServiceApi;
        _cache = cache;
    }

    public async Task<PagedResult<PositionDto>> GetPagedPositionsAsync(PositionQueryParameters queryParameters)
    {
        var cacheKey = GenerateCacheKey(queryParameters);

        if (_cache.TryGetValue(cacheKey, out PagedResult<PositionDto> cachedResult))
        {
            return cachedResult;
        }

        var result = await _interviewServiceApi.GetPagedPositionsAsync(queryParameters);

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(CacheDurationInMinutes));

        return result;
    }

    public async Task<Guid> CreatePositionAsync(CreatePositionDto model)
    {
        var id = await _interviewServiceApi.CreatePositionAsync(model);

        Interlocked.Increment(ref _version);

        return id;
    }

    private string GenerateCacheKey(PositionQueryParameters parameters)
    {
        return $"positions:v{_version}:" +
               $"page={parameters.Page}&" +
               $"size={parameters.PageSize}&" +
               $"title={parameters.TitleFilter?.ToLower().Trim()}&" +
               $"active={parameters.IsActive}";
    }
}
