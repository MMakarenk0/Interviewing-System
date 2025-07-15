using BFF.API.Models;
using Refit;

namespace BFF.API.Clients;

public interface IInterviewServiceApi
{
    [Get("/api/Position/paged")]
    Task<PagedResult<PositionDto>> GetPagedPositionsAsync([Query] PositionQueryParameters parameters);

    [Post("/api/Position")]
    Task<Guid> CreatePositionAsync([Body] CreatePositionDto dto);
}
