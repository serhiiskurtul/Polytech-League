using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.ArmwrestlingDto;
using PolytechLeague.Application.Services.Interfaces.Armwrestling;

namespace PolytechLeague.API.Controllers.Armwrestling;

[ApiController]
[Route("api/armwrestling-matches")]
public class ArmwrestlingMatchesController(IArmwrestlingMatchService matchService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var entities = await matchService.GetAllAsync(cancellationToken: cancellationToken);
        return Ok(entities.Select(e => e.ToResponse()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await matchService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ArmwrestlingMatchRequest request, CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await matchService.CreateAsync(entity, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] ArmwrestlingMatchRequest request, CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await matchService.UpdateAsync(id, entity, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await matchService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
