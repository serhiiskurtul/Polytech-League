using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.FootballDto;
using PolytechLeague.Application.Services.Interfaces.Football;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Football;

[ApiController]
[Route("api/football-teams")]
public class FootballTeamsController(
    IFootballTeamService FootballTeamService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<FootballTeamResponse>>("Football_teams_all");
        if (cached != null) return Ok(cached);

        var entities = await FootballTeamService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("Football_teams_all", response, TimeSpan.FromMinutes(60), "Football_teams_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await FootballTeamService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] FootballTeamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await FootballTeamService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("Football_teams_group");
        await redisService.RemoveGroupAsync("Football_events_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] FootballTeamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await FootballTeamService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("Football_teams_group");
        await redisService.RemoveGroupAsync("Football_events_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await FootballTeamService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("Football_teams_group");
        await redisService.RemoveGroupAsync("Football_events_group");

        return NoContent();
    }
}
