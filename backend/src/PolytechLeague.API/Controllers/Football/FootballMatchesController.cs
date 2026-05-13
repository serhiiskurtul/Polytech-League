using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.FootballDto;
using PolytechLeague.Application.Services.Interfaces.Football;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Football;

[ApiController]
[Route("api/Football-matches")]
public class FootballMatchesController(
    IFootballMatchService FootballMatchService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<FootballMatchResponse>>("Football_matches_all");
        if (cached != null) return Ok(cached);

        var entities = await FootballMatchService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("Football_matches_all", response, TimeSpan.FromMinutes(60), "Football_matches_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await FootballMatchService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] FootballMatchRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await FootballMatchService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("Football_matches_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] FootballMatchRequest request,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[CONTROLLER] Updating match {id}. Yellow: {request.YellowCards?.Count ?? 0}, Red: {request.RedCards?.Count ?? 0}");
        if (request.YellowCards != null) {
            foreach(var c in request.YellowCards) Console.WriteLine($"  - Yellow for player: {c.PlayerId}");
        }

        var entity = request.FromRequest();
        await FootballMatchService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("Football_matches_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await FootballMatchService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("Football_matches_group");

        return NoContent();
    }
}
