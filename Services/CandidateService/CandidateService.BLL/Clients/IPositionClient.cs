using CandidateService.BLL.Models;
using Refit;

namespace CandidateService.BLL.Clients;

public interface IPositionClient
{
    [Get("/api/position/{id}")]
    Task<PositionDto> GetPositionByIdAsync(Guid id);
}

