using PolytechLeague.Application.Dto.TableTennisDto;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Domain.Params;
using PolytechLeague.Domain.Entities.TableTennis;
using PolytechLeague.Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PolytechLeague.API.Controllers.TableTennis;

[ApiController]
[Route("api/table-tennis-teams")]
public class TableTennisTeamsController(ITableTennisTeamService ttTeamService, IRedisService redisService) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] FilterParams? filterParams,
        [FromQuery] SortParams? sortParams,
        [FromQuery] PageParams? pageParams,
        CancellationToken cancellationToken)
    {
        var result = await ttTeamService.GetPageAsync(filterParams, sortParams, pageParams, cancellationToken);
        var response = new PageResult<TableTennisTeamResponse>(
            result.Data.Select(t => t.ToResponse()).ToList(),
            result.TotalCount);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var entity = await ttTeamService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] TableTennisTeam request,
        CancellationToken cancellationToken)
    {
        await ttTeamService.CreateAsync(request, cancellationToken);
        
        await redisService.RemoveGroupAsync("tt_events_all");
        await redisService.RemoveDataAsync($"tt_event_v2_{request.EventId}");
        
        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        [FromRoute] long id,
        [FromBody] TableTennisTeam request,
        CancellationToken cancellationToken)
    {
        await ttTeamService.UpdateAsync(id, request, cancellationToken);
        
        await redisService.RemoveGroupAsync("tt_events_all");
        await redisService.RemoveDataAsync($"tt_event_v2_{request.EventId}");
        
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
    {
        var entity = await ttTeamService.GetByIdAsync(id, cancellationToken);
        await ttTeamService.DeleteAsync(id, cancellationToken);
        
        await redisService.RemoveGroupAsync("tt_events_all");
        await redisService.RemoveDataAsync($"tt_event_v2_{entity.EventId}");
        
        return NoContent();
    }
}
