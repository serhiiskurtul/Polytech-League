using PolytechLeague.Application.Dto.CheckersDto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Application.Services.Interfaces.Checkers;
using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Domain.Params;
using PolytechLeague.Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PolytechLeague.API.Controllers.Checkers;

[ApiController]
[Route("api/checkers-events")]
public class CheckersEventsController(ICheckersEventService checkersEventService, IRedisService redisService, IFileService fileService) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] FilterParams? filterParams,
        [FromQuery] SortParams? sortParams,
        [FromQuery] PageParams? pageParams,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"checkers_events_" +
                       $"{filterParams?.ToCacheKey() ?? "nofilter"}_" +
                       $"{sortParams?.ToCacheKey() ?? "nosort"}_" +
                       $"{pageParams?.ToCacheKey() ?? "nopage"}";

        var cached = await redisService.GetDataAsync<PageResult<CheckersEventResponse>>(cacheKey);
        if (cached is not null) return Ok(cached);

        var result = await checkersEventService.GetPageAsync(filterParams, sortParams, pageParams, cancellationToken);

        var response = new PageResult<CheckersEventResponse>(
            result.Data.Select(e => e.ToResponse()).ToList(),
            result.TotalCount);

        await redisService.SetDataAsync(cacheKey, response, TimeSpan.FromHours(24), "checkers_events_all");

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var cacheKey = $"checkers_event_{id}";

        var cached = await redisService.GetDataAsync<CheckersEventResponse>(cacheKey);
        if (cached is not null) return Ok(cached);

        var entity = await checkersEventService.GetByIdAsync(id, cancellationToken);
        var response = entity.ToResponse();

        await redisService.SetDataAsync(cacheKey, response, TimeSpan.FromHours(24), "checkers_events_byid");

        return Ok(response);
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CheckersEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await checkersEventService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("checkers_events_all");
        await redisService.RemoveGroupAsync("tournaments_group");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        [FromRoute] long id,
        [FromBody] CheckersEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await checkersEventService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("checkers_events_all");
        await redisService.RemoveDataAsync($"checkers_event_{id}");
        await redisService.RemoveGroupAsync("tournaments_group");
        await redisService.RemoveGroupAsync("tournaments_byid_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
    {
        await checkersEventService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("checkers_events_all");
        await redisService.RemoveDataAsync($"checkers_event_{id}");
        await redisService.RemoveGroupAsync("tournaments_group");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{eventId:long}/participants")]
    public async Task<IActionResult> AddParticipant([FromRoute] long eventId, [FromBody] CheckersEventParticipantDto request, CancellationToken cancellationToken)
    {
        await checkersEventService.AddParticipantAsync(eventId, request, cancellationToken);
        await redisService.RemoveDataAsync($"checkers_event_{eventId}");
        await redisService.RemoveGroupAsync("checkers_events_all");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{eventId:long}/participants/{playerId:long}")]
    public async Task<IActionResult> RemoveParticipant([FromRoute] long eventId, [FromRoute] long playerId, CancellationToken cancellationToken)
    {
        await checkersEventService.RemoveParticipantAsync(eventId, playerId, cancellationToken);
        await redisService.RemoveDataAsync($"checkers_event_{eventId}");
        await redisService.RemoveGroupAsync("checkers_events_all");
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{eventId:long}/participants/{playerId:long}")]
    public async Task<IActionResult> UpdateParticipant([FromRoute] long eventId, [FromRoute] long playerId, [FromBody] CheckersEventParticipantDto request, CancellationToken cancellationToken)
    {
        await checkersEventService.UpdateParticipantAsync(eventId, playerId, request, cancellationToken);
        await redisService.RemoveDataAsync($"checkers_event_{eventId}");
        await redisService.RemoveGroupAsync("checkers_events_all");
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/photos")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadPhoto([FromRoute] long id, [FromForm] IFormFile photo, CancellationToken cancellationToken)
    {
        if (photo == null || photo.Length == 0) return BadRequest("File is empty");

        var url = await fileService.SaveFileAsync(photo, $"tournaments/{id}", cancellationToken);
        await checkersEventService.AddPhotoAsync(id, url, cancellationToken);
        
        await redisService.RemoveDataAsync($"checkers_event_{id}");
        await redisService.RemoveGroupAsync("checkers_events_all");

        return Ok(new { url });
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}/photos/{photoId:long}")]
    public async Task<IActionResult> DeletePhoto(long id, long photoId, CancellationToken cancellationToken)
    {
        await checkersEventService.RemovePhotoAsync(id, photoId, cancellationToken);
        
        await redisService.RemoveDataAsync($"checkers_event_{id}");
        await redisService.RemoveGroupAsync("checkers_events_all");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/import")]
    public async Task<IActionResult> ImportFromChessResults(long id, [FromQuery] string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(url)) return BadRequest("URL is required");

        try
        {
            await checkersEventService.ImportDataFromUrlAsync(id, url, cancellationToken);

            await redisService.RemoveDataAsync($"checkers_event_{id}");
            await redisService.RemoveGroupAsync("checkers_events_all");

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/import-file")]
    public async Task<IActionResult> ImportFromFile(long id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0) return BadRequest("File is required");

        try
        {
            using var stream = file.OpenReadStream();
            await checkersEventService.ImportDataFromFileAsync(id, stream, cancellationToken);

            await redisService.RemoveDataAsync($"checkers_event_{id}");
            await redisService.RemoveGroupAsync("checkers_events_all");

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpPost("{id}/register")]
    public async Task<IActionResult> Register(long id, [FromQuery] long playerId, CancellationToken cancellationToken)
    {
        await checkersEventService.RegisterPlayerAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("checkers_events_all");
        await redisService.RemoveDataAsync($"checkers_event_{id}");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants/{playerId}/approve")]
    public async Task<IActionResult> Approve(long id, long playerId, CancellationToken cancellationToken)
    {
        await checkersEventService.ApproveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("checkers_events_all");
        await redisService.RemoveDataAsync($"checkers_event_{id}");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants/{playerId}/reject")]
    public async Task<IActionResult> Reject(long id, long playerId, CancellationToken cancellationToken)
    {
        await checkersEventService.RejectParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("checkers_events_all");
        await redisService.RemoveDataAsync($"checkers_event_{id}");
        return Ok();
    }
}
