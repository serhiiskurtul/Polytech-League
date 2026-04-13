using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.ChessDto;
using PolytechLeague.Application.Services.Interfaces.Chess;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Chess;

[ApiController]
[Route("api/chess-teams")]
public class ChessTeamsController(
    IChessTeamService chessTeamService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<ChessTeamResponse>>("chess_teams_all");
        if (cached != null) return Ok(cached);

        var entities = await chessTeamService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("chess_teams_all", response, TimeSpan.FromMinutes(60), "chess_teams_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await chessTeamService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] ChessTeamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await chessTeamService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveDataAsync($"chess_event_{request.EventId}");
        await redisService.RemoveGroupAsync("chess_events_byid");
        await redisService.RemoveGroupAsync("chess_events_all");
        await redisService.RemoveGroupAsync("chess_teams_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] ChessTeamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await chessTeamService.UpdateAsync(id, entity, cancellationToken);

        var team = await chessTeamService.GetByIdAsync(id, cancellationToken);
        await redisService.RemoveDataAsync($"chess_event_{team.EventId}");
        await redisService.RemoveGroupAsync("chess_events_byid");
        await redisService.RemoveGroupAsync("chess_events_all");
        await redisService.RemoveGroupAsync("chess_teams_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var team = await chessTeamService.GetByIdAsync(id, cancellationToken);
        await chessTeamService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveDataAsync($"chess_event_{team.EventId}");
        await redisService.RemoveGroupAsync("chess_events_byid");
        await redisService.RemoveGroupAsync("chess_events_all");
        await redisService.RemoveGroupAsync("chess_teams_group");

        return NoContent();
    }
}
