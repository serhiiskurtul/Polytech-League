using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.BadmintonDto;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Infrastructure.Redis;

namespace PolytechLeague.API.Controllers.Badminton;

[ApiController]
[Route("api/badminton-matches")]
public class BadmintonMatchesController(
    IBadmintonMatchService badmintonMatchService,
    IRedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var entities = await badmintonMatchService.GetAllAsync(cancellationToken: cancellationToken);
        var response = entities.Select(e => e.ToResponse()).ToList();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await badmintonMatchService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] BadmintonMatchRequest request,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[CREATE MATCH] EventId: {request.EventId}, Home: {request.HomeTeamId}, Away: {request.AwayTeamId}, Score: {request.HomeScore}:{request.AwayScore}");
        var entity = request.FromRequest();
        await badmintonMatchService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("badminton_events_group");

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] BadmintonMatchRequest request,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[UPDATE MATCH] Id: {id}, Home: {request.HomeTeamId}, Away: {request.AwayTeamId}, Score: {request.HomeScore}:{request.AwayScore}");
        var entity = request.FromRequest();
        await badmintonMatchService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("badminton_events_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await badmintonMatchService.DeleteAsync(id, cancellationToken);
        await redisService.RemoveGroupAsync("badminton_events_group");
        return NoContent();
    }
}
