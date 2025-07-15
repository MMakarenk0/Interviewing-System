using BFF.API.Models;

namespace BFF.API.Services.Interfaces;

public interface IPositionService
{
    Task<Guid> CreatePositionAsync(CreatePositionDto model);
    Task<PagedResult<PositionDto>> GetPagedPositionsAsync(PositionQueryParameters queryParameters);
}