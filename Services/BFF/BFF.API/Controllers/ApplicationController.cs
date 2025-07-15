using BFF.API.Models;
using BFF.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BFF.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateApplication([FromForm] CreateApplicationDto dto)
    {
        var id = await _applicationService.CreateApplicationAsync(dto);
        return CreatedAtAction(nameof(CreateApplication), new { id }, null);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var application = await _applicationService.GetApplicationByIdAsync(id);
        return Ok(application);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _applicationService.GetAllApplicationsAsync();
        return Ok(list);
    }

    [HttpGet("positions/{positionId}/applications")]
    public async Task<IActionResult> GetByPositionId(Guid positionId)
    {
        var applications = await _applicationService.GetApplicationsByPositionIdAsync(positionId);
        return Ok(applications);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateApplication([FromForm] CreateApplicationDto dto)
    {
        var id = await _applicationService.UpdateApplicationAsync(dto);
        return Ok(id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _applicationService.DeleteApplicationAsync(id);
        return NoContent();
    }

    [HttpPost("approve/{id}")]
    public async Task<IActionResult> Approve(Guid id)
    {
        await _applicationService.ApproveApplicationAsync(id);
        return NoContent();
    }
}
