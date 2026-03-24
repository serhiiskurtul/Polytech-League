using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.FutsalDto;
using PolytechLeague.Application.Services.Interfaces.Futsal;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Futsal;

[ApiController]
[Route("futsal-events")]
public class FutsalEventsController(
    IFutsalEventService futsalEventService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<FutsalEventResponse>>("futsal_events_all");
        if (cached != null) return Ok(cached);

        var entities = await futsalEventService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("futsal_events_all", response, TimeSpan.FromMinutes(60), "futsal_events_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await futsalEventService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] FutsalEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await futsalEventService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("futsal_events_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] FutsalEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await futsalEventService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("futsal_events_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await futsalEventService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("futsal_events_group");

        return NoContent();
    }
}
