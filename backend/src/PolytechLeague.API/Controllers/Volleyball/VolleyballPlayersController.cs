using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.VolleyballDto;
using PolytechLeague.Application.Services.Interfaces.Volleyball;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Volleyball;

[ApiController]
[Route("volleyball-players")]
public class VolleyballPlayersController(
    IVolleyballPlayerService volleyballPlayerService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<VolleyballPlayerResponse>>("volleyball_players_all");
        if (cached != null) return Ok(cached);

        var entities = await volleyballPlayerService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("volleyball_players_all", response, TimeSpan.FromMinutes(60), "volleyball_players_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await volleyballPlayerService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] VolleyballPlayerRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await volleyballPlayerService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("volleyball_players_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] VolleyballPlayerRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await volleyballPlayerService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("volleyball_players_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await volleyballPlayerService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("volleyball_players_group");

        return NoContent();
    }
}
