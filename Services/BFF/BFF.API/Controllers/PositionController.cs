using BFF.API.Models;
using BFF.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BFF.API.Controllers;
[EnableRateLimiting("Fixed")]
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

    [HttpPost]
    public async Task<IActionResult> CreatePosition([FromBody] CreatePositionDto dto)
    {
        var id = await _positionService.CreatePositionAsync(dto);
        return CreatedAtAction(nameof(CreatePosition), new { id }, null);
    }
}
