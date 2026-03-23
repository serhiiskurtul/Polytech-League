using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.ArmwrestlingDto;
using PolytechLeague.Application.Services.Interfaces.Armwrestling;

namespace PolytechLeague.API.Controllers.Armwrestling;

[ApiController]
[Route("armwrestling-players")]
public class ArmwrestlingPlayersController(IArmwrestlingPlayerService playerService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var entities = await playerService.GetAllAsync(cancellationToken: cancellationToken);
        return Ok(entities.Select(e => e.ToResponse()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await playerService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ArmwrestlingPlayerRequest request, CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await playerService.CreateAsync(entity, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity.ToResponse());
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] ArmwrestlingPlayerRequest request, CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await playerService.UpdateAsync(id, entity, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await playerService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
