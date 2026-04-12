using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.VolleyballDto;
using PolytechLeague.Application.Services.Interfaces.Volleyball;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Volleyball;

[ApiController]
[Route("api/volleyball-teams")]
public class VolleyballTeamsController(
    IVolleyballTeamService volleyballTeamService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<VolleyballTeamResponse>>("volleyball_teams_all");
        if (cached != null) return Ok(cached);

        var entities = await volleyballTeamService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("volleyball_teams_all", response, TimeSpan.FromMinutes(60), "volleyball_teams_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await volleyballTeamService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] VolleyballTeamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await volleyballTeamService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("volleyball_teams_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] VolleyballTeamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await volleyballTeamService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("volleyball_teams_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await volleyballTeamService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("volleyball_teams_group");

        return NoContent();
    }
}
