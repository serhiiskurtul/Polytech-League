using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.BeachVolleyballDto;
using PolytechLeague.Application.Services.Interfaces.BeachVolleyball;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.BeachVolleyball;

[ApiController]
[Route("beach-volleyball-events")]
public class BeachVolleyballEventsController(
    IBeachVolleyballEventService beachVolleyballEventService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<BeachVolleyballEventResponse>>("beach_volleyball_events_all");
        if (cached != null) return Ok(cached);

        var entities = await beachVolleyballEventService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("beach_volleyball_events_all", response, TimeSpan.FromMinutes(60), "beach_volleyball_events_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await beachVolleyballEventService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] BeachVolleyballEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await beachVolleyballEventService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("beach_volleyball_events_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] BeachVolleyballEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await beachVolleyballEventService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("beach_volleyball_events_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await beachVolleyballEventService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("beach_volleyball_events_group");

        return NoContent();
    }
}
