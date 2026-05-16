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
            return await _context.ExternalAchievements.OrderByDescending(e => e.Date).ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ExternalAchievement>> CreateEntry(
            [FromForm] string studentName,
            [FromForm] string title,
            [FromForm] string content,
            [FromForm] DateTime date,
            [FromForm] string? externalLink,
            IFormFile? file)
        {
            string? photoUrl = null;
            if (file != null)
            {
                photoUrl = await _fileService.SaveFileAsync(file, "external-achievements", default);
            }

            var entry = new ExternalAchievement
            {
                StudentName = studentName,
                Title = title,
                Content = content,
                Date = date,
                ExternalLink = externalLink,
                PhotoUrl = photoUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.ExternalAchievements.Add(entry);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEntries), new { id = entry.Id }, entry);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEntry(long id)
        {
            var entry = await _context.ExternalAchievements.FindAsync(id);
            if (entry == null) return NotFound();

            // Delete file if exists
            if (!string.IsNullOrEmpty(entry.PhotoUrl))
            {
                try { _fileService.DeleteFile(entry.PhotoUrl); } catch { /* ignore */ }
            }

            _context.ExternalAchievements.Remove(entry);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
