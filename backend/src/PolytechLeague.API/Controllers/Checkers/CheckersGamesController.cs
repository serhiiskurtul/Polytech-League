using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.CheckersDto;
using PolytechLeague.Application.Services.Interfaces.Checkers;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Checkers;

[ApiController]
[Route("api/checkers-games")]
public class CheckersGamesController(ICheckersGameService checkersGameService, IRedisService redisService) : Controller
{
    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CheckersGameDto request, CancellationToken cancellationToken)
    {
        var entity = request.FromDto();
        await checkersGameService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveDataAsync($"checkers_event_{request.TournamentId}");
        await redisService.RemoveGroupAsync("checkers_events_all");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] CheckersGameDto request, CancellationToken cancellationToken)
    {
        var entity = request.FromDto();
        entity.Id = id; 
        await checkersGameService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveDataAsync($"checkers_event_{request.TournamentId}");
        await redisService.RemoveGroupAsync("checkers_events_all");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id, [FromQuery] long tournamentId, CancellationToken cancellationToken)
    {
        await checkersGameService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveDataAsync($"checkers_event_{tournamentId}");
        await redisService.RemoveGroupAsync("checkers_events_all");

        return NoContent();
    }
}
