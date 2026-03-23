using PolytechLeague.Application.Dto.TableTennisDto;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Domain.Params;
using PolytechLeague.Domain.Entities.TableTennis;
using PolytechLeague.Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PolytechLeague.API.Controllers.TableTennis;

[ApiController]
[Route("table-tennis-events")]
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
        var cacheKey = $"tt_events_" +
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
        var cacheKey = $"tt_event_{id}";

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
        await redisService.RemoveDataAsync($"tt_event_{id}");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
    {
        await ttEventService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("tt_events_all");
        await redisService.RemoveDataAsync($"tt_event_{id}");

        return NoContent();
    }
}
