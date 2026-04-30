using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace PolytechLeague.API.Controllers.Common;

[ApiController]
[Route("api/event-media")]
public class EventMediaController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IFileService _fileService;

    public EventMediaController(ApplicationDbContext dbContext, IFileService fileService)
    {
        _dbContext = dbContext;
        _fileService = fileService;
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{eventId}/upload-multiple")]
    [DisableRequestSizeLimit]
    [RequestSizeLimit(2147483648)] // 2GB
    public async Task<IActionResult> UploadMultiple(long eventId, [FromForm] IEnumerable<IFormFile> files, CancellationToken cancellationToken)
    {
        var @event = await _dbContext.Events.FindAsync(new object[] { eventId }, cancellationToken);
        if (@event == null) return NotFound("Event not found");

        var uploadedUrls = new List<string>();

        try
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Determine folder based on file type
                    string folder = file.ContentType.StartsWith("video/") ? "events/videos" : "events/photos";
                    var url = await _fileService.SaveFileAsync(file, folder, cancellationToken);
                    
                    var eventPhoto = new EventPhotos
                    {
                        Url = url,
                        EventId = eventId
                    };

                    _dbContext.EventPhotos.Add(eventPhoto);
                    uploadedUrls.Add(url);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Ok(new { urls = uploadedUrls });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Помилка при завантаженні файлів", error = ex.Message });
        }
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{photoId}")]
    public async Task<IActionResult> DeleteMedia(long photoId, CancellationToken cancellationToken)
    {
        var photo = await _dbContext.EventPhotos.FindAsync(new object[] { photoId }, cancellationToken);
        if (photo == null) return NotFound();

        try
        {
            _fileService.DeleteFile(photo.Url);
        }
        catch { /* ignore if file not found */ }

        _dbContext.EventPhotos.Remove(photo);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{eventId}/set-main-photo")]
    [DisableRequestSizeLimit]
    [RequestSizeLimit(2147483648)] // 2GB
    public async Task<IActionResult> SetMainPhoto(long eventId, IFormFile file, CancellationToken cancellationToken)
    {
        var @event = await _dbContext.Events.FindAsync(new object[] { eventId }, cancellationToken);
        if (@event == null) return NotFound("Event not found");

        // Only delete the old photo if it was a dedicated main photo upload (not a link to a gallery photo)
        if (!string.IsNullOrEmpty(@event.MainPhotoUrl) && @event.MainPhotoUrl.Contains("/events/main/"))
        {
            try
            {
                _fileService.DeleteFile(@event.MainPhotoUrl);
            }
            catch { /* ignore */ }
        }

        var url = await _fileService.SaveFileAsync(file, "events/main", cancellationToken);
        
        @event.MainPhotoUrl = url;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new { url });
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("{eventId}/mark-as-main/{photoId}")]
    public async Task<IActionResult> MarkAsMain(long eventId, long photoId, CancellationToken cancellationToken)
    {
        var @event = await _dbContext.Events.FindAsync(new object[] { eventId }, cancellationToken);
        if (@event == null) return NotFound("Event not found");

        // Clean up old main photo if it was a dedicated upload
        if (!string.IsNullOrEmpty(@event.MainPhotoUrl) && @event.MainPhotoUrl.Contains("/events/main/"))
        {
            try { _fileService.DeleteFile(@event.MainPhotoUrl); } catch { }
        }

        if (photoId <= 0)
        {
            @event.MainPhotoUrl = null;
        }
        else
        {
            var photo = await _dbContext.EventPhotos.FindAsync(new object[] { photoId }, cancellationToken);
            if (photo == null || photo.EventId != eventId) return NotFound("Photo not found");
            @event.MainPhotoUrl = photo.Url;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(new { url = @event.MainPhotoUrl });
    }
}
