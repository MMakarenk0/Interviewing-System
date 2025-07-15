using CandidateService.API.Extensions;
using CandidateService.BLL.Models;
using CandidateService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandidateService.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [Authorize(Roles = "Interviewer, Administrator")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplicationById(Guid id)
    {
        var application = await _applicationService.GetApplicationByIdAsync(id);
        return Ok(application);
    }

    [Authorize(Roles = "Interviewer, Administrator")]
    [HttpGet]
    public async Task<IActionResult> GetAllApplications()
    {
        var applications = await _applicationService.GetAllApplicationsAsync();
        return Ok(applications);
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("positions/{positionId}/applications")]
    public async Task<IActionResult> GetApplicationsByPositionId(Guid positionId)
    {
        var applications = await _applicationService.GetApplicationsByPositionIdAsync(positionId);
        return Ok(applications);
    }

    [Authorize(Roles = "Candidate, Administrator")]
    [HttpPost]
    public async Task<IActionResult> CreateApplication([FromForm] CreateApplicationDto applicationModel)
    {
        if (applicationModel == null)
            return BadRequest("Invalid application data");

        var applicationId = await _applicationService.CreateApplicationAsync(applicationModel);
        return CreatedAtAction(nameof(GetApplicationById), new { id = applicationId }, null);
    }

    [Authorize(Roles = "Candidate, Administrator")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApplication(Guid id)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        var isOwner = await _applicationService.IsApplicationOwnedByUserAsync(id, userId.Value);
        if (!User.IsAdmin() && !isOwner)
            return Forbid();

        await _applicationService.DeleteApplicationAsync(id);
        return NoContent();
    }


    [Authorize(Roles = "Candidate, Administrator")]
    [HttpPut]
    public async Task<IActionResult> UpdateApplication([FromForm] CreateApplicationDto applicationModel)
    {
        if (applicationModel == null)
            return BadRequest("Invalid application data");

        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        var isOwner = await _applicationService.IsApplicationOwnedByUserAsync(applicationModel.Id, userId.Value);
        if (!User.IsAdmin() && !isOwner)
            return Forbid();

        var updatedId = await _applicationService.UpdateApplicationAsync(applicationModel);
        return Ok(updatedId);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("approve/{id}")]
    public async Task<IActionResult> ApproveApplication(Guid id)
    {
        await _applicationService.ApproveApplicationAsync(id);
        return NoContent();
    }
}
