using InterviewService.API.Extensions;
using InterviewService.BLL.Models;
using InterviewService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewService.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class InterviewController : ControllerBase
{
    private readonly IInterviewService _interviewService;
    public InterviewController(IInterviewService interviewService)
    {
        _interviewService = interviewService;
    }

    [Authorize(Roles = "Interviewer, Administrator")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInterviewById(Guid id)
    {
        var interview = await _interviewService.GetInterviewByIdAsync(id);
        return Ok(interview);
    }

    [Authorize(Roles = "Interviewer, Administrator")]
    [HttpGet]
    public async Task<IActionResult> GetAllInterviews()
    {
        var interviews = await _interviewService.GetAllInterviewsAsync();
        return Ok(interviews);
    }

    [Authorize(Roles = "Interviewer, Administrator")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInterview(Guid id)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        var isOwner = await _interviewService.IsInterviewOwnedByUserAsync(id, userId.Value);
        if (!User.IsAdmin() && !isOwner)
            return Forbid();

        await _interviewService.DeleteInterviewAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Interviewer, Administrator")]
    [HttpPost]
    public async Task<IActionResult> CreateInterview([FromBody] CreateInterviewDto model)
    {
        if (model == null)
            return BadRequest("Invalid interview data");

        var interviewId = await _interviewService.CreateInterviewAsync(model);
        return CreatedAtAction(nameof(GetInterviewById), new { id = interviewId }, null);
    }

    [Authorize(Roles = "Interviewer, Administrator")]
    [HttpPut]
    public async Task<IActionResult> UpdateInterview([FromBody] CreateInterviewDto model)
    {
        if (model == null)
            return BadRequest("Invalid interview data");

        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        var isOwner = await _interviewService.IsInterviewOwnedByUserAsync(model.Id, userId.Value);
        if (!User.IsAdmin() && !isOwner)
            return Forbid();

        var updatedId = await _interviewService.UpdateInterviewAsync(model);
        return Ok(new { Id = updatedId });
    }
}
