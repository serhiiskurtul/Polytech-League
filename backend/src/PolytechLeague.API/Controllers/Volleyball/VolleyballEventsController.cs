using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.VolleyballDto;
using PolytechLeague.Application.Services.Interfaces.Volleyball;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Volleyball;

[ApiController]
[Route("api/volleyball-events")]
public class VolleyballEventsController(
    IVolleyballEventService volleyballEventService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<VolleyballEventResponse>>("volleyball_events_all");
        if (cached != null) return Ok(cached);

        var entities = await volleyballEventService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("volleyball_events_all", response, TimeSpan.FromMinutes(60), "volleyball_events_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await volleyballEventService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] VolleyballEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await volleyballEventService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("volleyball_events_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] VolleyballEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await volleyballEventService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("volleyball_events_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await volleyballEventService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("volleyball_events_group");

        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/register")]
    public async Task<IActionResult> Register(long id, [FromQuery] long playerId, CancellationToken cancellationToken)
    {
        await volleyballEventService.RegisterPlayerAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("volleyball_events_group");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants/{playerId}/approve")]
    public async Task<IActionResult> ApproveParticipant(long id, long playerId, CancellationToken cancellationToken)
    {
        await volleyballEventService.ApproveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("volleyball_events_group");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants/{playerId}/reject")]
    public async Task<IActionResult> RejectParticipant(long id, long playerId, CancellationToken cancellationToken)
    {
        await volleyballEventService.RejectParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("volleyball_events_group");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}/participants/{playerId}")]
    public async Task<IActionResult> RemoveParticipant(long id, long playerId, CancellationToken cancellationToken)
    {
        await volleyballEventService.RemoveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("volleyball_events_group");
        return NoContent();
    }
}
