using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.FutsalDto;
using PolytechLeague.Application.Services.Interfaces.Futsal;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Futsal;

[ApiController]
[Route("futsal-players")]
public class FutsalPlayersController(
    IFutsalPlayerService futsalPlayerService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<FutsalPlayerResponse>>("futsal_players_all");
        if (cached != null) return Ok(cached);

        var entities = await futsalPlayerService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("futsal_players_all", response, TimeSpan.FromMinutes(60), "futsal_players_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await futsalPlayerService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] FutsalPlayerRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await futsalPlayerService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("futsal_players_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] FutsalPlayerRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await futsalPlayerService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("futsal_players_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await futsalPlayerService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("futsal_players_group");

        return NoContent();
    }
}
