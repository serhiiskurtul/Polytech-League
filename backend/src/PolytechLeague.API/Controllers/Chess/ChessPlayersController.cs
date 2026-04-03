using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using PolytechLeague.Application.Dto.ChessDto;
using PolytechLeague.Application.Services.Interfaces.Chess;

namespace PolytechLeague.API.Controllers.Chess;

[ApiController]
[Route("chess-players")]
public class ChessPlayersController(IChessPlayerService chessPlayerService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var entities = await chessPlayerService.GetAllAsync(cancellationToken: cancellationToken);
        return Ok(entities.Select(e => e.ToDto()));
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ChessPlayerDto request, CancellationToken cancellationToken)
    {
        var entity = request.FromDto();
        var created = await chessPlayerService.CreateAsync(entity, cancellationToken);
        return Ok(created.ToDto());
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] ChessPlayerDto request, CancellationToken cancellationToken)
    {
        var entity = request.FromDto();
        entity.Id = id;
        await chessPlayerService.UpdateAsync(id, entity, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
    {
        await chessPlayerService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
