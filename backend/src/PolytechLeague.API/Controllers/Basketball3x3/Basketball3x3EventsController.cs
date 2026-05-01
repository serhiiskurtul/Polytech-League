using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.Basketball3x3Dto;
using PolytechLeague.Application.Services.Interfaces.Basketball3x3;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Basketball3x3;

[ApiController]
[Route("api/basketball3x3-events")]
public class Basketball3x3EventsController(
    IBasketball3x3EventService basketball3x3EventService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<Basketball3x3EventResponse>>("basketball3x3_events_all");
        if (cached != null) return Ok(cached);

        var entities = await basketball3x3EventService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("basketball3x3_events_all", response, TimeSpan.FromMinutes(60), "basketball3x3_events_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await basketball3x3EventService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] Basketball3x3EventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await basketball3x3EventService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("basketball3x3_events_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] Basketball3x3EventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await basketball3x3EventService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("basketball3x3_events_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await basketball3x3EventService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("basketball3x3_events_group");

        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/register")]
    public async Task<IActionResult> Register(long id, [FromQuery] long playerId, CancellationToken cancellationToken)
    {
        await basketball3x3EventService.RegisterPlayerAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("basketball3x3_events_group");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants/{playerId}/approve")]
    public async Task<IActionResult> ApproveParticipant(long id, long playerId, CancellationToken cancellationToken)
    {
        await basketball3x3EventService.ApproveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("basketball3x3_events_group");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants/{playerId}/reject")]
    public async Task<IActionResult> RejectParticipant(long id, long playerId, CancellationToken cancellationToken)
    {
        await basketball3x3EventService.RejectParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("basketball3x3_events_group");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}/participants/{playerId}")]
    public async Task<IActionResult> RemoveParticipant(long id, long playerId, CancellationToken cancellationToken)
    {
        await basketball3x3EventService.RemoveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("basketball3x3_events_group");
        return NoContent();
    }
}
