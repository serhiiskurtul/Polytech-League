using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.ChessDto;
using PolytechLeague.Application.Services.Interfaces.Chess;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Chess;

[ApiController]
[Route("chess-games")]
public class ChessGamesController(IChessGameService chessGameService, IRedisService redisService) : Controller
{
    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ChessGameDto request, CancellationToken cancellationToken)
    {
        var entity = request.FromDto();
        await chessGameService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveDataAsync($"chess_event_{request.TournamentId}");
        await redisService.RemoveGroupAsync("chess_events_all");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] ChessGameDto request, CancellationToken cancellationToken)
    {
        var entity = request.FromDto();
        entity.Id = id; // ensure ID matches
        await chessGameService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveDataAsync($"chess_event_{request.TournamentId}");
        await redisService.RemoveGroupAsync("chess_events_all");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id, [FromQuery] long tournamentId, CancellationToken cancellationToken)
    {
        await chessGameService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveDataAsync($"chess_event_{tournamentId}");
        await redisService.RemoveGroupAsync("chess_events_all");

        return NoContent();
    }
}
