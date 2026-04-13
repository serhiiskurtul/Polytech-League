using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.Basketball3x3Dto;
using PolytechLeague.Application.Services.Interfaces.Basketball3x3;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Basketball3x3;

[ApiController]
[Route("api/basketball3x3-teams")]
public class Basketball3x3TeamsController(
    IBasketball3x3TeamService basketball3x3TeamService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<Basketball3x3TeamResponse>>("basketball3x3_teams_all");
        if (cached != null) return Ok(cached);

        var entities = await basketball3x3TeamService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("basketball3x3_teams_all", response, TimeSpan.FromMinutes(60), "basketball3x3_teams_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await basketball3x3TeamService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] Basketball3x3TeamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await basketball3x3TeamService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("basketball3x3_teams_group");
        await redisService.RemoveGroupAsync("basketball3x3_events_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] Basketball3x3TeamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await basketball3x3TeamService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("basketball3x3_teams_group");
        await redisService.RemoveGroupAsync("basketball3x3_events_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await basketball3x3TeamService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("basketball3x3_teams_group");
        await redisService.RemoveGroupAsync("basketball3x3_events_group");

        return NoContent();
    }
}
