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
            }

            if (userId.HasValue)
            {
                var userEntity = await _dbContext.Users.FindAsync(userId.Value);
                if (userEntity != null)
                {
                    userEntity.LastVisitAt = DateTime.UtcNow;
                }
            }

            await _dbContext.SaveChangesAsync();

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

        [HttpGet("daily-stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DailyVisitStats>>> GetDailyStats()
        {
            var dailyStats = await _dbContext.SiteVisits
                .GroupBy(v => v.VisitDate.Date)
                .Select(g => new DailyVisitStats
                {
                    Date = g.Key,
                    TotalVisits = g.Count(),
                    RegisteredVisits = g.Count(v => v.UserId != null),
                    GuestVisits = g.Count(v => v.UserId == null)
                })
                .OrderByDescending(s => s.Date)
                .Take(30)
                .ToListAsync();

            return Ok(dailyStats);
        }

        [HttpGet("users-by-date")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<VisitedUserDto>>> GetUsersByDate([FromQuery] DateTime date)
        {
            var targetDate = date.Date;
            
            var users = await _dbContext.SiteVisits
                .Where(v => v.VisitDate.Date == targetDate && v.UserId != null)
                .Select(v => v.User)
                .Distinct()
                .Select(u => new VisitedUserDto
                {
                    Id = u!.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    LastVisitAt = u.LastVisitAt
                })
                .ToListAsync();

            return Ok(users);
        }
    }

    public class VisitedUserDto
    {
        public long Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Email { get; set; }
        public required string Role { get; set; }
        public DateTime? LastVisitAt { get; set; }
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

    public class DailyVisitStats
    {
        public DateTime Date { get; set; }
        public long TotalVisits { get; set; }
        public long RegisteredVisits { get; set; }
        public long GuestVisits { get; set; }
    }
}
