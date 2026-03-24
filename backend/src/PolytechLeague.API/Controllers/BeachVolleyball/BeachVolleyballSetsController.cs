using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.BeachVolleyballDto;
using PolytechLeague.Application.Services.Interfaces.BeachVolleyball;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.BeachVolleyball;

[ApiController]
[Route("beach-volleyball-sets")]
public class BeachVolleyballSetsController(
    IBeachVolleyballSetService beachVolleyballSetService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cached = await redisService.GetDataAsync<IEnumerable<BeachVolleyballSetResponse>>("beach_volleyball_sets_all");
        if (cached != null) return Ok(cached);

        var entities = await beachVolleyballSetService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();

        await redisService.SetDataAsync("beach_volleyball_sets_all", response, TimeSpan.FromMinutes(60), "beach_volleyball_sets_group");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await beachVolleyballSetService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] BeachVolleyballSetRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await beachVolleyballSetService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("beach_volleyball_sets_group");
        await redisService.RemoveGroupAsync("beach_volleyball_matches_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] BeachVolleyballSetRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await beachVolleyballSetService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("beach_volleyball_sets_group");
        await redisService.RemoveGroupAsync("beach_volleyball_matches_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await beachVolleyballSetService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("beach_volleyball_sets_group");
        await redisService.RemoveGroupAsync("beach_volleyball_matches_group");

        return NoContent();
    }
}
