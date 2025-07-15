using InterviewService.BLL.Models;

namespace InterviewService.BLL.Services.Interfaces;

public interface IPositionService
{
    Task<Guid> CreatePositionAsync(CreatePositionDto positionDto);
    Task<PagedResult<PositionDto>> GetPagedPositionsAsync(PositionQueryParameters parameters);
    Task<PositionDto> GetPositionByIdAsync(Guid id);
}