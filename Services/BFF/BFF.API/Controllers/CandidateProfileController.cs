using BFF.API.Models;
using BFF.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BFF.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CandidateProfileController : ControllerBase
{
    private readonly ICandidateProfileService _candidateProfileService;

    public CandidateProfileController(ICandidateProfileService candidateProfileService)
    {
        _candidateProfileService = candidateProfileService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCandidateProfileById(Guid id)
    {
        var profile = await _candidateProfileService.GetCandidateProfileByIdAsync(id);
        return Ok(profile);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCandidateProfiles()
    {
        var profiles = await _candidateProfileService.GetAllCandidateProfilesAsync();
        return Ok(profiles);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCandidateProfile([FromForm] CreateCandidateProfileDto candidateProfileModel)
    {
        if (candidateProfileModel == null)
            return BadRequest("Invalid candidate profile data");

        var profileId = await _candidateProfileService.CreateCandidateProfileAsync(candidateProfileModel);
        return CreatedAtAction(nameof(GetCandidateProfileById), new { id = profileId }, null);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCandidateProfile(Guid id)
    {
        await _candidateProfileService.DeleteCandidateProfileAsync(id);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCandidateProfile([FromForm] CreateCandidateProfileDto candidateProfileModel)
    {
        if (candidateProfileModel == null)
            return BadRequest("Invalid candidate profile data");

        var updatedId = await _candidateProfileService.UpdateCandidateProfileAsync(candidateProfileModel);
        return Ok(updatedId);
    }
}
