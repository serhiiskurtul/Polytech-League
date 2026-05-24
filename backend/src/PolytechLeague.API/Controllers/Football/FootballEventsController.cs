using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.FootballDto;
using PolytechLeague.Application.Services.Interfaces.Football;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Football;

[ApiController]
[Route("api/football-events")]
public class FootballEventsController(
    IFootballEventService FootballEventService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<FootballEventResponse>>("Football_events_all");
        if (cached != null) return Ok(cached);

        var entities = await FootballEventService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("Football_events_all", response, TimeSpan.FromMinutes(60), "Football_events_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await FootballEventService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] FootballEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await FootballEventService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("Football_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] FootballEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await FootballEventService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("Football_events_group");
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        await redisService.RemoveGroupAsync("tournaments_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await FootballEventService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("Football_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("tournaments_byid_group");

        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/register")]
    public async Task<IActionResult> Register(long id, [FromQuery] long playerId, CancellationToken cancellationToken)
    {
        await FootballEventService.RegisterPlayerAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("Football_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants/{playerId}/approve")]
    public async Task<IActionResult> ApproveParticipant(long id, long playerId, CancellationToken cancellationToken)
    {
        await FootballEventService.ApproveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("Football_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants/{playerId}/reject")]
    public async Task<IActionResult> RejectParticipant(long id, long playerId, CancellationToken cancellationToken)
    {
        await FootballEventService.RejectParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("Football_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}/participants/{playerId}")]
    public async Task<IActionResult> RemoveParticipant(long id, long playerId, CancellationToken cancellationToken)
    {
        await FootballEventService.RemoveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("Football_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return NoContent();
    }
}
