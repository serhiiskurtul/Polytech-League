using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.BadmintonDto;
using PolytechLeague.Application.Services.Interfaces.Badminton;

namespace PolytechLeague.API.Controllers.Badminton;

[ApiController]
[Route("badminton-players")]
public class BadmintonPlayersController(
    IBadmintonPlayerService badmintonPlayerService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var entities = await badmintonPlayerService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await badmintonPlayerService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await badmintonPlayerService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
