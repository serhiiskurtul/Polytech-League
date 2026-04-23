using PolytechLeague.Application.Dto.ChessDto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Application.Services.Interfaces.Chess;
using PolytechLeague.Domain.Params;
using PolytechLeague.Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PolytechLeague.API.Controllers.Chess;

[ApiController]
[Route("api/chess-events")]
public class ChessEventsController(IChessEventService chessEventService, IRedisService redisService, IFileService fileService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("ping")]
    public IActionResult Ping() => Ok("pong");

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] FilterParams? filterParams,
        [FromQuery] SortParams? sortParams,
        [FromQuery] PageParams? pageParams,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"chess_events_" +
                       $"{filterParams?.ToCacheKey() ?? "nofilter"}_" +
                       $"{sortParams?.ToCacheKey() ?? "nosort"}_" +
                       $"{pageParams?.ToCacheKey() ?? "nopage"}";

        var cached = await redisService.GetDataAsync<PageResult<ChessEventResponse>>(cacheKey);
        if (cached is not null) return Ok(cached);

        var result = await chessEventService.GetPageAsync(filterParams, sortParams, pageParams, cancellationToken);

        var response = new PageResult<ChessEventResponse>(
            result.Data.Select(e => e.ToResponse()).ToList(),
            result.TotalCount);

        await redisService.SetDataAsync(cacheKey, response, TimeSpan.FromMinutes(60), "chess_events_all");

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var cacheKey = $"chess_event_{id}";

        var cached = await redisService.GetDataAsync<ChessEventResponse>(cacheKey);
        if (cached is not null) return Ok(cached);

        var entity = await chessEventService.GetByIdAsync(id, cancellationToken);
        var response = entity.ToResponse();

        await redisService.SetDataAsync(cacheKey, response, TimeSpan.FromMinutes(60), "chess_events_byid");

        return Ok(response);
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] ChessEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await chessEventService.CreateAsync(entity, cancellationToken);

        await redisService.RemoveGroupAsync("chess_events_all");

        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        [FromRoute] long id,
        [FromBody] ChessEventRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await chessEventService.UpdateAsync(id, entity, cancellationToken);

        await redisService.RemoveGroupAsync("chess_events_all");
        await redisService.RemoveDataAsync($"chess_event_{id}");

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
    {
        await chessEventService.DeleteAsync(id, cancellationToken);

        await redisService.RemoveGroupAsync("chess_events_all");
        await redisService.RemoveDataAsync($"chess_event_{id}");

        return NoContent();
    }
    [Authorize(Policy = "Admin")]
    [HttpPost("{eventId:long}/participants")]
    public async Task<IActionResult> AddParticipant([FromRoute] long eventId, [FromBody] ChessEventParticipantDto request, CancellationToken cancellationToken)
    {
        await chessEventService.AddParticipantAsync(eventId, request, cancellationToken);
        await redisService.RemoveDataAsync($"chess_event_{eventId}");
        await redisService.RemoveGroupAsync("chess_events_all");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{eventId:long}/participants/{playerId:long}")]
    public async Task<IActionResult> RemoveParticipant([FromRoute] long eventId, [FromRoute] long playerId, CancellationToken cancellationToken)
    {
        await chessEventService.RemoveParticipantAsync(eventId, playerId, cancellationToken);
        await redisService.RemoveDataAsync($"chess_event_{eventId}");
        await redisService.RemoveGroupAsync("chess_events_all");
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{eventId:long}/participants/{playerId:long}")]
    public async Task<IActionResult> UpdateParticipant([FromRoute] long eventId, [FromRoute] long playerId, [FromBody] ChessEventParticipantDto request, CancellationToken cancellationToken)
    {
        await chessEventService.UpdateParticipantAsync(eventId, playerId, request, cancellationToken);
        await redisService.RemoveDataAsync($"chess_event_{eventId}");
        await redisService.RemoveGroupAsync("chess_events_all");
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/photos")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadPhoto([FromRoute] long id, [FromForm] IFormFile photo, CancellationToken cancellationToken)
    {
        if (photo == null || photo.Length == 0) return BadRequest("File is empty");

        var url = await fileService.SaveFileAsync(photo, $"tournaments/{id}", cancellationToken);
        await chessEventService.AddPhotoAsync(id, url, cancellationToken);
        
        await redisService.RemoveDataAsync($"chess_event_{id}");
        await redisService.RemoveGroupAsync("chess_events_all");

        return Ok(new { url });
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}/photos/{photoId:long}")]
    public async Task<IActionResult> DeletePhoto(long id, long photoId, CancellationToken cancellationToken)
    {
        // First get the photo to know the URL to delete from disk (optional, or just delete from DB)
        // I'll keep the file on disk for now to avoid complexity of finding URL, or I could modify the service.
        // Let's just delete from DB for now as per simple implementation.
        
        await chessEventService.RemovePhotoAsync(id, photoId, cancellationToken);
        
        await redisService.RemoveDataAsync($"chess_event_{id}");
        await redisService.RemoveGroupAsync("chess_events_all");

        return NoContent();
    }
    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/import")]
    public async Task<IActionResult> ImportFromChessResults(long id, [FromQuery] string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(url)) return BadRequest("URL is required");

        try
        {
            await chessEventService.ImportDataFromUrlAsync(id, url, cancellationToken);

            await redisService.RemoveDataAsync($"chess_event_{id}");
            await redisService.RemoveGroupAsync("chess_events_all");

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
            await chessEventService.ImportDataFromFileAsync(id, stream, cancellationToken);

            await redisService.RemoveDataAsync($"chess_event_{id}");
            await redisService.RemoveGroupAsync("chess_events_all");

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
        await chessEventService.RegisterPlayerAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("chess_events_all");
        await redisService.RemoveDataAsync($"chess_event_{id}");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants/{playerId}/approve")]
    public async Task<IActionResult> Approve(long id, long playerId, CancellationToken cancellationToken)
    {
        await chessEventService.ApproveParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("chess_events_all");
        await redisService.RemoveDataAsync($"chess_event_{id}");
        return Ok();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{id}/participants/{playerId}/reject")]
    public async Task<IActionResult> Reject(long id, long playerId, CancellationToken cancellationToken)
    {
        await chessEventService.RejectParticipantAsync(id, playerId, cancellationToken);
        await redisService.RemoveGroupAsync("chess_events_all");
        await redisService.RemoveDataAsync($"chess_event_{id}");
        return Ok();
    }
}
