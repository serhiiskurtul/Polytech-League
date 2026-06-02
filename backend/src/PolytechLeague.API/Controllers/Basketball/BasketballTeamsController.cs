using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.BasketballDto;
using PolytechLeague.Application.Services.Interfaces.Basketball;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Basketball;

[ApiController]
[Route("api/basketball3x3-teams")]
public class BasketballTeamsController(
    IBasketballTeamService basketball3x3TeamService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<BasketballTeamResponse>>("basketball_teams_all");
        if (cached != null) return Ok(cached);

        var entities = await basketball3x3TeamService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("basketball_teams_all", response, TimeSpan.FromMinutes(60), "basketball_teams_group");
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
        [FromBody] BasketballTeamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await basketball3x3TeamService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("basketball_teams_group");
        await redisService.RemoveGroupAsync("basketball_events_group");

        return Created();
    }

    [Authorize(Policy = "AdminOrFizorg")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] BasketballTeamRequest request,
        [FromServices] PolytechLeague.Application.Services.Interfaces.ITeamAuthorizationService authService,
        CancellationToken cancellationToken)
    {
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (userRole == "Fizorg")
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (long.TryParse(userIdStr, out long userId))
            {
                var team = await basketball3x3TeamService.GetByIdAsync(id, cancellationToken);
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
        await basketball3x3TeamService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("basketball_teams_group");
        await redisService.RemoveGroupAsync("basketball_events_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await basketball3x3TeamService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("basketball_teams_group");
        await redisService.RemoveGroupAsync("basketball_events_group");

        return NoContent();
    }
}
