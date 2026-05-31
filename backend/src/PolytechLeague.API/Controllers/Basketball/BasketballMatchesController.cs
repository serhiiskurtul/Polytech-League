using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.BasketballDto;
using PolytechLeague.Application.Services.Interfaces.Basketball;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Basketball;

[ApiController]
[Route("api/basketball3x3-matches")]
public class BasketballMatchesController(
    IBasketballMatchService basketball3x3MatchService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<BasketballMatchResponse>>("basketball3x3_matches_all");
        if (cached != null) return Ok(cached);

        var entities = await basketball3x3MatchService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("basketball3x3_matches_all", response, TimeSpan.FromMinutes(60), "basketball3x3_matches_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await basketball3x3MatchService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] BasketballMatchRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await basketball3x3MatchService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("basketball3x3_matches_group");
        await redisService.RemoveGroupAsync("basketball3x3_events_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] BasketballMatchRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await basketball3x3MatchService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("basketball3x3_matches_group");
        await redisService.RemoveGroupAsync("basketball3x3_events_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await basketball3x3MatchService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("basketball3x3_matches_group");
        await redisService.RemoveGroupAsync("basketball3x3_events_group");

        return NoContent();
    }
}
