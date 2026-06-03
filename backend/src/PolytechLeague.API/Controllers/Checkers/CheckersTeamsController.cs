using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.CheckersDto;
using PolytechLeague.Application.Services.Interfaces.Checkers;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Checkers;

[ApiController]
[Route("api/checkers-teams")]
public class CheckersTeamsController(
    ICheckersTeamService checkersTeamService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<CheckersTeamResponse>>("checkers_teams_all");
        if (cached != null) return Ok(cached);

        var entities = await checkersTeamService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("checkers_teams_all", response, TimeSpan.FromMinutes(60), "checkers_teams_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await checkersTeamService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "AdminOrFizorg")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CheckersTeamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await checkersTeamService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveDataAsync($"checkers_event_{request.EventId}");
        await redisService.RemoveGroupAsync("checkers_events_byid");
        await redisService.RemoveGroupAsync("checkers_events_all");
        await redisService.RemoveGroupAsync("checkers_teams_group");

        return Created();
    }

    [Authorize(Policy = "AdminOrFizorg")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] CheckersTeamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await checkersTeamService.UpdateAsync(id, entity, cancellationToken);

        var team = await checkersTeamService.GetByIdAsync(id, cancellationToken);
        await redisService.RemoveDataAsync($"checkers_event_{team.EventId}");
        await redisService.RemoveGroupAsync("checkers_events_byid");
        await redisService.RemoveGroupAsync("checkers_events_all");
        await redisService.RemoveGroupAsync("checkers_teams_group");

        return NoContent();
    }

    [Authorize(Policy = "AdminOrFizorg")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var team = await checkersTeamService.GetByIdAsync(id, cancellationToken);
        await checkersTeamService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveDataAsync($"checkers_event_{team.EventId}");
        await redisService.RemoveGroupAsync("checkers_events_byid");
        await redisService.RemoveGroupAsync("checkers_events_all");
        await redisService.RemoveGroupAsync("checkers_teams_group");

        return NoContent();
    }
}
