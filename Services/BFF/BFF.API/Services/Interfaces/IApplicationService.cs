using BFF.API.Models;

namespace BFF.API.Services.Interfaces;

public interface IApplicationService
{
    Task ApproveApplicationAsync(Guid id);
    Task<Guid> CreateApplicationAsync(CreateApplicationDto dto);
    Task DeleteApplicationAsync(Guid id);
    Task<List<ApplicationDto>> GetAllApplicationsAsync();
    Task<ApplicationDto> GetApplicationByIdAsync(Guid id);
    Task<List<ApplicationDto>> GetApplicationsByPositionIdAsync(Guid positionId);
    Task<Guid> UpdateApplicationAsync(CreateApplicationDto dto);
}