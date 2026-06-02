using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Application.Dto.AthleticsDto;
using PolytechLeague.Application.Services.Interfaces.Athletics;
using PolytechLeague.Domain.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Athletics;

[ApiController]
[Route("api/athletics-events")]
public class AthleticsEventsController(
    IAthleticsEventService athleticsEventService,
    IRedisService redisService, IPlayerService playerService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] FilterParams? filterParams,
        [FromQuery] SortParams? sortParams,
        [FromQuery] PageParams? pageParams,
        CancellationToken cancellationToken)
    {
        var result = await athleticsEventService.GetPageAsync(filterParams, sortParams, pageParams, cancellationToken);
        var response = new PageResult<AthleticsEventResponse>(
            result.Data.Select(e => e.ToResponse()).ToList(),
            result.TotalCount);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var entity = await athleticsEventService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] AthleticsEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await athleticsEventService.CreateAsync(entity, cancellationToken);
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        [FromRoute] long id,
        [FromBody] AthleticsEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await athleticsEventService.UpdateAsync(id, entity, cancellationToken);
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
    {
        await athleticsEventService.DeleteAsync(id, cancellationToken);
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id:long}/results")]
    public async Task<IActionResult> AddResult([FromRoute] long id, [FromBody] AthleticsResultDto dto, CancellationToken cancellationToken)
    {
        await athleticsEventService.AddResultAsync(id, dto, cancellationToken);
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:long}/results/{resultId:long}")]
    public async Task<IActionResult> UpdateResult([FromRoute] long id, [FromRoute] long resultId, [FromBody] AthleticsResultDto dto, CancellationToken cancellationToken)
    {
        await athleticsEventService.UpdateResultAsync(id, resultId, dto, cancellationToken);
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}/results/{resultId:long}")]
    public async Task<IActionResult> DeleteResult([FromRoute] long id, [FromRoute] long resultId, CancellationToken cancellationToken)
    {
        await athleticsEventService.RemoveResultAsync(id, resultId, cancellationToken);
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id:long}/participants")]
    public async Task<IActionResult> AddParticipant([FromRoute] long id, [FromBody] AthleticsEventParticipantDto dto, CancellationToken cancellationToken)
    {
        await athleticsEventService.AddParticipantAsync(id, dto, cancellationToken);
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}/participants/{playerId:long}")]
    public async Task<IActionResult> RemoveParticipant([FromRoute] long id, [FromRoute] long playerId, CancellationToken cancellationToken)
    {
        await athleticsEventService.RemoveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("rankings_group");
        playerService.ClearRankingsCache();
        await redisService.RemoveGroupAsync("tournaments_byid_group");
        return NoContent();
    }
}


