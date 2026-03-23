using PolytechLeague.Application.Dto.CheckersDto;
using PolytechLeague.Application.Services.Interfaces.Checkers;
using PolytechLeague.Domain.Params;
using PolytechLeague.Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PolytechLeague.API.Controllers.Checkers;

[ApiController]
[Route("checkers-events")]
public class CheckersEventsController(ICheckersEventService checkersEventService, IRedisService redisService) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] FilterParams? filterParams,
        [FromQuery] SortParams? sortParams,
        [FromQuery] PageParams? pageParams,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"checkers_events_" +
                       $"{filterParams?.ToCacheKey() ?? "nofilter"}_" +
                       $"{sortParams?.ToCacheKey() ?? "nosort"}_" +
                       $"{pageParams?.ToCacheKey() ?? "nopage"}";

        var cached = await redisService.GetDataAsync<PageResult<CheckersEventResponse>>(cacheKey);
        if (cached is not null) return Ok(cached);

        var result = await checkersEventService.GetPageAsync(filterParams, sortParams, pageParams, cancellationToken);

        var response = new PageResult<CheckersEventResponse>(
            result.Data.Select(e => e.ToResponse()).ToList(),
            result.TotalCount);

        await redisService.SetDataAsync(cacheKey, response, TimeSpan.FromMinutes(60), "checkers_events_all");

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var cacheKey = $"checkers_event_{id}";

        var cached = await redisService.GetDataAsync<CheckersEventResponse>(cacheKey);
        if (cached is not null) return Ok(cached);

        var entity = await checkersEventService.GetByIdAsync(id, cancellationToken);
        var response = entity.ToResponse();

        await redisService.SetDataAsync(cacheKey, response, TimeSpan.FromMinutes(60), "checkers_events_byid");

        return Ok(response);
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CheckersEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await checkersEventService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("checkers_events_all");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        [FromRoute] long id,
        [FromBody] CheckersEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await checkersEventService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("checkers_events_all");
        await redisService.RemoveDataAsync($"checkers_event_{id}");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
    {
        await checkersEventService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("checkers_events_all");
        await redisService.RemoveDataAsync($"checkers_event_{id}");

        return NoContent();
    }
}
