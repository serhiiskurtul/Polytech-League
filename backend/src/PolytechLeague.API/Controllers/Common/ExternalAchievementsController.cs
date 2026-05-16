using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PolytechLeague.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace PolytechLeague.API.Controllers.Common
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalAchievementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public ExternalAchievementsController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExternalAchievement>>> GetEntries()
        {
            return await _context.ExternalAchievements
                .Include(e => e.Media)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ExternalAchievement>> CreateEntry(
            [FromForm] string studentName,
            [FromForm] string title,
            [FromForm] string content,
            [FromForm] DateTime date,
            [FromForm] string? externalLink,
            List<IFormFile>? photos,
            List<IFormFile>? videos)
        {
            var entry = new ExternalAchievement
            {
                StudentName = studentName,
                Title = title,
                Content = content,
                Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
                ExternalLink = externalLink,
                CreatedAt = DateTime.UtcNow
            };

            if (photos != null)
            {
                foreach (var photo in photos)
                {
                    var url = await _fileService.SaveFileAsync(photo, "external-achievements", default);
                    entry.Media.Add(new ExternalAchievementMedia { Url = url, Type = "photo" });
                }
            }

            if (videos != null)
            {
                foreach (var video in videos)
                {
                    var url = await _fileService.SaveFileAsync(video, "external-achievements", default);
                    entry.Media.Add(new ExternalAchievementMedia { Url = url, Type = "video" });
                }
            }

            _context.ExternalAchievements.Add(entry);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEntries), new { id = entry.Id }, entry);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEntry(
            long id,
            [FromForm] string studentName,
            [FromForm] string title,
            [FromForm] string content,
            [FromForm] DateTime date,
            [FromForm] string? externalLink,
            List<IFormFile>? photos,
            List<IFormFile>? videos)
        {
            var entry = await _context.ExternalAchievements
                .Include(e => e.Media)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entry == null) return NotFound();

            entry.StudentName = studentName;
            entry.Title = title;
            entry.Content = content;
            entry.Date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            entry.ExternalLink = externalLink;

            if (photos != null)
            {
                foreach (var photo in photos)
                {
                    var url = await _fileService.SaveFileAsync(photo, "external-achievements", default);
                    entry.Media.Add(new ExternalAchievementMedia { Url = url, Type = "photo" });
                }
            }

            if (videos != null)
            {
                foreach (var video in videos)
                {
                    var url = await _fileService.SaveFileAsync(video, "external-achievements", default);
                    entry.Media.Add(new ExternalAchievementMedia { Url = url, Type = "video" });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEntry(long id)
        {
            var entry = await _context.ExternalAchievements
                .Include(e => e.Media)
                .FirstOrDefaultAsync(e => e.Id == id);
                
            if (entry == null) return NotFound();

            // Delete files if exist
            foreach (var media in entry.Media)
            {
                try { _fileService.DeleteFile(media.Url); } catch { /* ignore */ }
            }

            _context.ExternalAchievements.Remove(entry);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
