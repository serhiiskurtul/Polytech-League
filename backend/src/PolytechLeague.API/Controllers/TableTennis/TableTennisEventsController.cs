using PolytechLeague.Application.Dto.TableTennisDto;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Domain.Params;
using PolytechLeague.Domain.Entities.TableTennis;
using PolytechLeague.Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PolytechLeague.API.Controllers.TableTennis;

[ApiController]
[Route("api/table-tennis-events")]
public class TableTennisEventsController(ITableTennisEventService ttEventService, IRedisService redisService) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] FilterParams? filterParams,
        [FromQuery] SortParams? sortParams,
        [FromQuery] PageParams? pageParams,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"tt_events_v2_" +
                       $"{filterParams?.ToCacheKey() ?? "nofilter"}_" +
                       $"{sortParams?.ToCacheKey() ?? "nosort"}_" +
                       $"{pageParams?.ToCacheKey() ?? "nopage"}";

        var cached = await redisService.GetDataAsync<PageResult<TableTennisEventResponse>>(cacheKey);
        if (cached is not null) return Ok(cached);

        var result = await ttEventService.GetPageAsync(filterParams, sortParams, pageParams, cancellationToken);

        var response = new PageResult<TableTennisEventResponse>(
            result.Data.Select(e => e.ToResponse()).ToList(),
            result.TotalCount);

        await redisService.SetDataAsync(cacheKey, response, TimeSpan.FromMinutes(60), "tt_events_all");

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var cacheKey = $"tt_event_v2_{id}";

        var cached = await redisService.GetDataAsync<TableTennisEventResponse>(cacheKey);
        if (cached is not null) return Ok(cached);

        var entity = await ttEventService.GetByIdAsync(id, cancellationToken);
        var response = entity.ToResponse();

        await redisService.SetDataAsync(cacheKey, response, TimeSpan.FromMinutes(60), "tt_events_byid");

        return Ok(response);
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] TableTennisEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await ttEventService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("tt_events_all");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        [FromRoute] long id,
        [FromBody] TableTennisEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await ttEventService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("tt_events_all");
        await redisService.RemoveDataAsync($"tt_event_v2_{id}");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
    {
        await ttEventService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("tt_events_all");
        await redisService.RemoveDataAsync($"tt_event_v2_{id}");

        return NoContent();
    }
    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants")]
    public async Task<IActionResult> AddParticipant(long id, [FromBody] TableTennisEventParticipantDto dto, CancellationToken cancellationToken)
    {
        await ttEventService.AddParticipantAsync(id, dto, cancellationToken);
        await redisService.RemoveGroupAsync("tt_events_all");
        await redisService.RemoveDataAsync($"tt_event_v2_{id}");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}/participants/{playerId}")]
    public async Task<IActionResult> UpdateParticipant(long id, long playerId, [FromBody] TableTennisEventParticipantDto dto, CancellationToken cancellationToken)
    {
        await ttEventService.UpdateParticipantAsync(id, playerId, dto, cancellationToken);
        await redisService.RemoveGroupAsync("tt_events_all");
        await redisService.RemoveDataAsync($"tt_event_v2_{id}");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}/participants/{playerId}")]
    public async Task<IActionResult> RemoveParticipant(long id, long playerId, CancellationToken cancellationToken)
    {
        await ttEventService.RemoveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("tt_events_all");
        await redisService.RemoveDataAsync($"tt_event_v2_{id}");
        return Ok();
    }

    [Authorize]
    [HttpPost("{id}/register")]
    public async Task<IActionResult> Register(long id, [FromQuery] long playerId, CancellationToken cancellationToken)
    {
        await ttEventService.RegisterPlayerAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("tt_events_all");
        await redisService.RemoveDataAsync($"tt_event_v2_{id}");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants/{playerId}/approve")]
    public async Task<IActionResult> Approve(long id, long playerId, CancellationToken cancellationToken)
    {
        await ttEventService.ApproveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("tt_events_all");
        await redisService.RemoveDataAsync($"tt_event_v2_{id}");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants/{playerId}/reject")]
    public async Task<IActionResult> Reject(long id, long playerId, CancellationToken cancellationToken)
    {
        await ttEventService.RejectParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("tt_events_all");
        await redisService.RemoveDataAsync($"tt_event_v2_{id}");
        return Ok();
    }
}
