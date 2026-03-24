using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.FutsalDto;
using PolytechLeague.Application.Services.Interfaces.Futsal;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Futsal;

[ApiController]
[Route("futsal-matches")]
public class FutsalMatchesController(
    IFutsalMatchService futsalMatchService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<FutsalMatchResponse>>("futsal_matches_all");
        if (cached != null) return Ok(cached);

        var entities = await futsalMatchService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("futsal_matches_all", response, TimeSpan.FromMinutes(60), "futsal_matches_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await futsalMatchService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] FutsalMatchRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await futsalMatchService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("futsal_matches_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] FutsalMatchRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await futsalMatchService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("futsal_matches_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await futsalMatchService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("futsal_matches_group");

        return NoContent();
    }
}
