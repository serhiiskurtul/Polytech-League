using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.ArmwrestlingDto;
using PolytechLeague.Application.Services.Interfaces.Armwrestling;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Armwrestling;

[ApiController]
[Route("api/armwrestling-matches")]
public class ArmwrestlingMatchesController(
    IArmwrestlingMatchService matchService, IRedisService redisService, IPlayerService playerService) : ControllerBase
{
    // Деталі турніру кешуються, тому після зміни поєдинків кеш треба скинути, інакше сторінка показує старі дані
    private async Task InvalidateCacheAsync()
    {
        await redisService.RemoveGroupAsync("arm_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
    }

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
        await InvalidateCacheAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] ArmwrestlingMatchRequest request, CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await matchService.UpdateAsync(id, entity, cancellationToken);
        await InvalidateCacheAsync();
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await matchService.DeleteAsync(id, cancellationToken);
        await InvalidateCacheAsync();
        return NoContent();
    }
}
