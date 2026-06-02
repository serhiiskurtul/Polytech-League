using PolytechLeague.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.ArmwrestlingDto;
using PolytechLeague.Application.Services.Interfaces.Armwrestling;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Armwrestling;

[ApiController]
[Route("api/armwrestling-events")]
public class ArmwrestlingEventsController(
    IArmwrestlingEventService armEventService,
    IRedisService redisService, IPlayerService playerService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<ArmwrestlingEventResponse>>("arm_events_all");
        if (cached != null) return Ok(cached);

        var entities = await armEventService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("arm_events_all", response, TimeSpan.FromHours(24), "arm_events_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var cacheKey = $"arm_event_{id}";
        var cached = await redisService.GetDataAsync<ArmwrestlingEventResponse>(cacheKey);
        if (cached != null) return Ok(cached);

        var entity = await armEventService.GetByIdAsync(id, cancellationToken);
        var response = entity.ToResponse();

        await redisService.SetDataAsync(cacheKey, response, TimeSpan.FromHours(24), "arm_events_group");
        return Ok(response);
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] ArmwrestlingEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await armEventService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("arm_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] ArmwrestlingEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await armEventService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("arm_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await armEventService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("arm_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id:long}/participants")]
    public async Task<IActionResult> AddParticipant(long id, [FromBody] ArmwrestlingEventParticipantDto dto, CancellationToken cancellationToken)
    {
        await armEventService.AddParticipantAsync(id, dto, cancellationToken);
        await redisService.RemoveGroupAsync("arm_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}/participants/{playerId:long}")]
    public async Task<IActionResult> RemoveParticipant(long id, long playerId, CancellationToken cancellationToken)
    {
        await armEventService.RemoveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("arm_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/register")]
    public async Task<IActionResult> Register(long id, [FromQuery] long playerId, CancellationToken cancellationToken)
    {
        await armEventService.RegisterPlayerAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("arm_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants/{playerId}/approve")]
    public async Task<IActionResult> Approve(long id, long playerId, CancellationToken cancellationToken)
    {
        await armEventService.ApproveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("arm_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants/{playerId}/reject")]
    public async Task<IActionResult> Reject(long id, long playerId, CancellationToken cancellationToken)
    {
        await armEventService.RejectParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("arm_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Ok();
    }
}


