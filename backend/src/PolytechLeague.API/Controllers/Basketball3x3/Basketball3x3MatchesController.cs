using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.Basketball3x3Dto;
using PolytechLeague.Application.Services.Interfaces.Basketball3x3;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Basketball3x3;

[ApiController]
[Route("api/basketball3x3-matches")]
public class Basketball3x3MatchesController(
    IBasketball3x3MatchService basketball3x3MatchService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<Basketball3x3MatchResponse>>("basketball3x3_matches_all");
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
        [FromBody] Basketball3x3MatchRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await basketball3x3MatchService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("basketball3x3_matches_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] Basketball3x3MatchRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await basketball3x3MatchService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("basketball3x3_matches_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await basketball3x3MatchService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("basketball3x3_matches_group");

        return NoContent();
    }
}
