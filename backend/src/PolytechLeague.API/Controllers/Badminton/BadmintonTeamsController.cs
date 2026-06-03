using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.BadmintonDto;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Badminton;

[ApiController]
[Route("api/badminton-teams")]
public class BadmintonTeamsController(
    IBadmintonTeamService badmintonTeamService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<BadmintonTeamResponse>>("badminton_teams_all");
        if (cached != null) return Ok(cached);

        var entities = await badmintonTeamService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("badminton_teams_all", response, TimeSpan.FromMinutes(60), "badminton_events_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await badmintonTeamService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "AdminOrFizorg")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] BadmintonTeamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.ToEntity();
        await badmintonTeamService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("badminton_events_group");
        
        var response = entity.ToResponse();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, response);
    }

    [Authorize(Policy = "AdminOrFizorg")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] BadmintonTeamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.ToEntity();
        await badmintonTeamService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("badminton_events_group");

        return NoContent();
    }

    [Authorize(Policy = "AdminOrFizorg")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await badmintonTeamService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("badminton_events_group");

        return NoContent();
    }
}
