using InterviewService.BLL.Models;
using InterviewService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InterviewService.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PositionController : ControllerBase
{
    private readonly IPositionService _positionService;

    public PositionController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedPositions([FromQuery] PositionQueryParameters parameters)
    {
        var pagedResult = await _positionService.GetPagedPositionsAsync(parameters);
        return Ok(pagedResult);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPositionById(Guid id)
    {
        var position = await _positionService.GetPositionByIdAsync(id);
        if (position == null)
            return NotFound($"Position with ID={id} not found");
        return Ok(position);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePosition([FromBody] CreatePositionDto positionDto)
    {
        if (positionDto == null)
            return BadRequest("Position data is required");
        var id = await _positionService.CreatePositionAsync(positionDto);
        return CreatedAtAction(nameof(GetPositionById), new { id }, null);
    }
}
