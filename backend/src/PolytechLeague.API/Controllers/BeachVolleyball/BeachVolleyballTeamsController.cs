using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.BeachVolleyballDto;
using PolytechLeague.Application.Services.Interfaces.BeachVolleyball;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.BeachVolleyball;

[ApiController]
[Route("api/beach-volleyball-teams")]
public class BeachVolleyballTeamsController(
    IBeachVolleyballTeamService beachVolleyballTeamService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<BeachVolleyballTeamResponse>>("beach_volleyball_teams_all");
        if (cached != null) return Ok(cached);

        var entities = await beachVolleyballTeamService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("beach_volleyball_teams_all", response, TimeSpan.FromMinutes(60), "beach_volleyball_teams_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await beachVolleyballTeamService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] BeachVolleyballTeamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await beachVolleyballTeamService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("beach_volleyball_teams_group");
        await redisService.RemoveGroupAsync("beachvolleyball_events_group");

        return Created();
    }

    [Authorize(Policy = "AdminOrFizorg")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] BeachVolleyballTeamRequest request,
        [FromServices] PolytechLeague.Application.Services.Interfaces.ITeamAuthorizationService authService,
        CancellationToken cancellationToken)
    {
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (userRole == "Fizorg")
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (long.TryParse(userIdStr, out long userId))
            {
                var team = await beachVolleyballTeamService.GetByIdAsync(id, cancellationToken);
                try {
                    await authService.ValidateFizorgAccessAsync(userId, team.Name, team.EventId, request.PlayerIds, cancellationToken);
                } catch (UnauthorizedAccessException) {
                    return Forbid();
                } catch (ArgumentException ex) {
                    return BadRequest(ex.Message);
                }
            }
        }

        var entity = request.FromRequest();
        await beachVolleyballTeamService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("beach_volleyball_teams_group");
        await redisService.RemoveGroupAsync("beachvolleyball_events_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await beachVolleyballTeamService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("beach_volleyball_teams_group");
        await redisService.RemoveGroupAsync("beachvolleyball_events_group");

        return NoContent();
    }
}
