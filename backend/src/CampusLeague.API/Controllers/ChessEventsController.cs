using CampusLeague.Application.Dto.ChessDto;
using CampusLeague.Application.Services.Interfaces.Chess;
using CampusLeague.Domain.Params;
using CampusLeague.Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("chess-events")]
public class ChessEventsController(IChessEventService chessEventService, IRedisService redisService) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] FilterParams? filterParams,
        [FromQuery] SortParams? sortParams,
        [FromQuery] PageParams? pageParams,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"chess_events_" +
                       $"{filterParams?.ToCacheKey() ?? "nofilter"}_" +
                       $"{sortParams?.ToCacheKey() ?? "nosort"}_" +
                       $"{pageParams?.ToCacheKey() ?? "nopage"}";

        var cached = await redisService.GetDataAsync<PageResult<ChessEventResponse>>(cacheKey);
        if (cached is not null) return Ok(cached);

        var result = await chessEventService.GetPageAsync(filterParams, sortParams, pageParams, cancellationToken);

        var response = new PageResult<ChessEventResponse>(
            result.Data.Select(e => e.ToResponse()).ToList(),
            result.TotalCount);

        await redisService.SetDataAsync(cacheKey, response, TimeSpan.FromMinutes(60), "chess_events_all");

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var cacheKey = $"chess_event_{id}";

        var cached = await redisService.GetDataAsync<ChessEventResponse>(cacheKey);
        if (cached is not null) return Ok(cached);

        var entity = await chessEventService.GetByIdAsync(id, cancellationToken);
        var response = entity.ToResponse();

        await redisService.SetDataAsync(cacheKey, response, TimeSpan.FromMinutes(60), "chess_events_byid");

        return Ok(response);
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] ChessEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await chessEventService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("chess_events_all");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        [FromRoute] long id,
        [FromBody] ChessEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await chessEventService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("chess_events_all");
        await redisService.RemoveDataAsync($"chess_event_{id}");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
    {
        await chessEventService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("chess_events_all");
        await redisService.RemoveDataAsync($"chess_event_{id}");

        return NoContent();
    }
}
