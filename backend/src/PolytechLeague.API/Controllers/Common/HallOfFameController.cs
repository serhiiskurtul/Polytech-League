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

namespace PolytechLeague.API.Controllers.Common
{
    [ApiController]
    [Route("api/[controller]")]
    public class HallOfFameController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public HallOfFameController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HallOfFameManualEntry>>> GetEntries([FromQuery] string? sport = null)
        {
            var query = _context.HallOfFameManualEntries.AsQueryable();
            if (!string.IsNullOrEmpty(sport))
            {
                query = query.Where(e => e.Sport == sport);
            }
            return await query.ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HallOfFameManualEntry>> CreateEntry(
            [FromForm] string sport,
            [FromForm] string seasonName,
            [FromForm] string teamName,
            IFormFile? file)
        {
            string? photoUrl = null;
            if (file != null)
            {
                photoUrl = await _fileService.SaveFileAsync(file, "hall-of-fame", default);
            }

            var entry = new HallOfFameManualEntry
            {
                Sport = sport,
                SeasonName = seasonName,
                TeamName = teamName,
                PhotoUrl = photoUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.HallOfFameManualEntries.Add(entry);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEntries), new { sport = entry.Sport }, entry);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEntry(long id, HallOfFameManualEntry entry)
        {
            if (id != entry.Id) return BadRequest();

            _context.Entry(entry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntryExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEntry(long id)
        {
            var entry = await _context.HallOfFameManualEntries.FindAsync(id);
            if (entry == null) return NotFound();

            // Delete file if exists
            if (!string.IsNullOrEmpty(entry.PhotoUrl))
            {
                try { _fileService.DeleteFile(entry.PhotoUrl); } catch { /* ignore */ }
            }

            _context.HallOfFameManualEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EntryExists(long id)
        {
            return _context.HallOfFameManualEntries.Any(e => e.Id == id);
        }
    }
}
