using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;
using System.Security.Claims;

namespace PolytechLeague.Api.Controllers.Common
{
    [ApiController]
    [Route("api/[controller]")]
    public class SiteVisitsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public SiteVisitsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> RecordVisit([FromBody] SiteVisitRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            long? userId = null;
            if (long.TryParse(userIdStr, out var parsed))
            {
                userId = parsed;
            }

            var today = DateTime.UtcNow.Date;

            // Optional: avoid inserting multiple records per day for the same user/guest
            bool exists = false;
            if (userId.HasValue)
            {
                exists = await _dbContext.SiteVisits.AnyAsync(v => v.UserId == userId.Value && v.VisitDate.Date == today);
            }
            else if (!string.IsNullOrEmpty(request.GuestId))
            {
                exists = await _dbContext.SiteVisits.AnyAsync(v => v.GuestId == request.GuestId && v.VisitDate.Date == today);
            }

            if (!exists)
            {
                var visit = new SiteVisit
                {
                    VisitDate = DateTime.UtcNow,
                    UserId = userId,
                    GuestId = string.IsNullOrEmpty(request.GuestId) ? Guid.NewGuid().ToString() : request.GuestId
                };

                _dbContext.SiteVisits.Add(visit);
                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SiteVisitStats>> GetStats()
        {
            var totalVisits = await _dbContext.SiteVisits.CountAsync();
            
            var uniqueRegisteredUsers = await _dbContext.SiteVisits
                .Where(v => v.UserId != null)
                .Select(v => v.UserId)
                .Distinct()
                .CountAsync();

            var uniqueUnregisteredUsers = await _dbContext.SiteVisits
                .Where(v => v.UserId == null && v.GuestId != null)
                .Select(v => v.GuestId)
                .Distinct()
                .CountAsync();

            return Ok(new SiteVisitStats
            {
                TotalVisits = totalVisits,
                UniqueRegisteredUsers = uniqueRegisteredUsers,
                UniqueUnregisteredUsers = uniqueUnregisteredUsers
            });
        }
    }

    public class SiteVisitRequest
    {
        public string? GuestId { get; set; }
    }

    public class SiteVisitStats
    {
        public long TotalVisits { get; set; }
        public long UniqueRegisteredUsers { get; set; }
        public long UniqueUnregisteredUsers { get; set; }
    }
}
