using PolytechLeague.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.BadmintonDto;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Badminton;

[ApiController]
[Route("api/badminton-events")]
public class BadmintonEventsController(
    IBadmintonEventService badmintonEventService,
    IRedisService redisService, IPlayerService playerService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<BadmintonEventResponse>>("badminton_events_all");
        if (cached != null) return Ok(cached);

        var entities = await badmintonEventService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("badminton_events_all", response, TimeSpan.FromHours(24), "badminton_events_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var cacheKey = $"badminton_event_{id}";
        var cached = await redisService.GetDataAsync<BadmintonEventResponse>(cacheKey);
        if (cached != null) return Ok(cached);

        var entity = await badmintonEventService.GetByIdAsync(id, cancellationToken);
        var response = entity.ToResponse();

        await redisService.SetDataAsync(cacheKey, response, TimeSpan.FromHours(24), "badminton_events_group");
        return Ok(response);
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] BadmintonEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await badmintonEventService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("badminton_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] BadmintonEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await badmintonEventService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("badminton_events_group");
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
        await badmintonEventService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("badminton_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");

        return NoContent();
    }
    [Authorize(Policy = "AdminOrFizorg")]
    [HttpPost("{id}/participants")]
    public async Task<IActionResult> AddParticipant(long id, [FromBody] BadmintonEventParticipantDto dto, CancellationToken cancellationToken)
    {
        await badmintonEventService.AddParticipantAsync(id, dto, cancellationToken);
        await redisService.RemoveGroupAsync("badminton_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Ok();
    }

    [Authorize(Policy = "AdminOrFizorg")]
    [HttpPut("{id}/participants/{playerId}")]
    public async Task<IActionResult> UpdateParticipant(long id, long playerId, [FromBody] BadmintonEventParticipantDto dto, CancellationToken cancellationToken)
    {
        await badmintonEventService.UpdateParticipantAsync(id, playerId, dto, cancellationToken);
        await redisService.RemoveGroupAsync("badminton_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Ok();
    }

    [Authorize(Policy = "AdminOrFizorg")]
    [HttpDelete("{id}/participants/{playerId}")]
    public async Task<IActionResult> RemoveParticipant(long id, long playerId, CancellationToken cancellationToken)
    {
        await badmintonEventService.RemoveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("badminton_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Ok();
    }

    [Authorize]
    [HttpPost("{id}/register")]
    public async Task<IActionResult> Register(long id, [FromQuery] long playerId, CancellationToken cancellationToken)
    {
        await badmintonEventService.RegisterPlayerAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("badminton_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Ok();
    }

    [Authorize(Policy = "AdminOrFizorg")]
    [HttpPost("{id}/participants/{playerId}/approve")]
    public async Task<IActionResult> Approve(long id, long playerId, CancellationToken cancellationToken)
    {
        await badmintonEventService.ApproveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("badminton_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Ok();
    }

    [Authorize(Policy = "AdminOrFizorg")]
    [HttpPost("{id}/participants/{playerId}/reject")]
    public async Task<IActionResult> Reject(long id, long playerId, CancellationToken cancellationToken)
    {
        await badmintonEventService.RejectParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("badminton_events_group");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Ok();
    }
}


