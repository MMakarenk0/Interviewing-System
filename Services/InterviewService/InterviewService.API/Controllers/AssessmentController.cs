using InterviewService.API.Extensions;
using InterviewService.BLL.Models;
using InterviewService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewService.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AssessmentController : ControllerBase
{
    private readonly IAssessmentService _assessmentService;
    private readonly IInterviewService _interviewService;

    public AssessmentController(IAssessmentService assessmentService, IInterviewService interviewService)
    {
        _assessmentService = assessmentService;
        _interviewService = interviewService;
    }

    [Authorize(Roles = "Interviewer, Administrator")]
    [HttpGet]
    public async Task<IActionResult> GetAllAssessments()
    {
        var assessments = await _assessmentService.GetAllAssessmentsAsync();
        return Ok(assessments);
    }

    [Authorize(Roles = "Interviewer, Administrator")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssessmentById(Guid id)
    {
        var assessment = await _assessmentService.GetAssessmentByIdAsync(id);
        if (assessment == null)
            return NotFound($"Assessment with ID={id} not found");
        return Ok(assessment);
    }

    [Authorize(Roles = "Interviewer, Administrator")]
    [HttpPost]
    public async Task<IActionResult> CreateAssessment([FromBody] CreateAssessmentDto model)
    {
        if (model == null)
            return BadRequest("Invalid assessment data");

        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        if (!User.IsAdmin())
        {
            var isOwner = await _interviewService.IsInterviewOwnedByUserAsync(model.InterviewId, userId.Value);
            if (!isOwner)
                return Forbid();
        }

        var id = await _assessmentService.CreateAssessmentAsync(model);
        return CreatedAtAction(nameof(GetAssessmentById), new { id }, null);
    }

}
