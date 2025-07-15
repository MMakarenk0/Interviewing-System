using CandidateService.API.Extensions;
using CandidateService.BLL.Models;
using CandidateService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandidateService.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CandidateProfileController : ControllerBase
{
    private readonly ICandidateProfileService _candidateProfileService;

    public CandidateProfileController(ICandidateProfileService candidateProfileService)
    {
        _candidateProfileService = candidateProfileService;
    }

    [Authorize(Roles = "Candidate, Interviewer, Administrator")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCandidateProfileById(Guid id)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        var isOwner = await _candidateProfileService.IsProfileOwnedByUserAsync(id, userId.Value);
        if (!User.IsAdmin() && !User.IsInterviewer() && !isOwner)
            return Forbid();

        var profile = await _candidateProfileService.GetCandidateProfileByIdAsync(id);
        return Ok(profile);
    }

    [Authorize(Roles = "Candidate, Interviewer, Administrator")]
    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetCandidateProfileByUserId(Guid userId)
    {
        var requesterId = User.GetUserId();
        if (requesterId == null)
            return Unauthorized();

        var isOwner = userId == requesterId;

        if (!User.IsAdmin() && !User.IsInterviewer() && !isOwner)
            return Forbid();

        var profile = await _candidateProfileService.GetCandidateProfileByUserIdAsync(userId);
        if (profile == null)
            return NotFound();

        return Ok(profile);
    }

    [HttpPost("by-ids")]
    public async Task<IActionResult> GetCandidateProfilesByIds([FromBody] IdListRequest request)
    {
        if (request?.Ids == null || !request.Ids.Any())
            return BadRequest("No candidate profile IDs provided");

        var profiles = await _candidateProfileService.GetCandidateProfilesByUserIds(request.Ids);
        return Ok(profiles);
    }

    [HttpPost("filter")]
    public async Task<ActionResult<List<CandidateProfileDto>>> FilterCandidateProfiles([FromBody] CandidateProfileQuery query)
    {
        var result = await _candidateProfileService.FilterCandidateProfilesAsync(query);
        return Ok(result);
    }


    [Authorize(Roles = "Interviewer, Administrator")]
    [HttpGet]
    public async Task<IActionResult> GetAllCandidateProfiles()
    {
        var profiles = await _candidateProfileService.GetAllCandidateProfilesAsync();
        return Ok(profiles);
    }

    [Authorize(Roles = "Candidate, Administrator")]
    [HttpPost]
    public async Task<IActionResult> CreateCandidateProfile([FromForm] CreateCandidateProfileDto candidateProfileModel)
    {
        if (candidateProfileModel == null)
            return BadRequest("Invalid candidate profile data");

        var profileId = await _candidateProfileService.CreateCandidateProfileAsync(candidateProfileModel);
        return CreatedAtAction(nameof(GetCandidateProfileById), new { id = profileId }, null);
    }

    [Authorize(Roles = "Candidate, Administrator")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCandidateProfile(Guid id)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        var isOwner = await _candidateProfileService.IsProfileOwnedByUserAsync(id, userId.Value);
        if (!User.IsAdmin() && !isOwner)
            return Forbid();

        await _candidateProfileService.DeleteCandidateProfileAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Candidate, Administrator")]
    [HttpPut]
    public async Task<IActionResult> UpdateCandidateProfile([FromForm] CreateCandidateProfileDto candidateProfileModel)
    {
        if (candidateProfileModel == null)
            return BadRequest("Invalid candidate profile data");

        var userId = User.GetUserId();
        if (userId == null)
            return Unauthorized();

        var isOwner = await _candidateProfileService.IsProfileOwnedByUserAsync(candidateProfileModel.Id, userId.Value);
        if (!User.IsAdmin() && !isOwner)
            return Forbid();

        var updatedId = await _candidateProfileService.UpdateCandidateProfileAsync(candidateProfileModel);
        return Ok(updatedId);
    }
}
