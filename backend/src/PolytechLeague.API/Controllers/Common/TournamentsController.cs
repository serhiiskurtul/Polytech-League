using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Infrastructure.Redis;


namespace PolytechLeague.API.Controllers.Common;

[ApiController]
[Route("api/[controller]")]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;
    private readonly IRedisService _redisService;

    public TournamentsController(ITournamentService tournamentService, IRedisService redisService)
    {
        _tournamentService = tournamentService;
        _redisService = redisService;
    }


    [HttpGet]
    public async Task<ActionResult<PagedResponse<TournamentDto>>> GetAllTournaments(
        int page = 1, 
        int pageSize = 10,
        string? status = null,
        string? sport = null,
        string? level = null)
    {
        var cacheKey = $"tournaments_p{page}_s{pageSize}_{status ?? "all"}_{sport ?? "all"}_{level ?? "all"}";
        var cached = await _redisService.GetDataAsync<PagedResponse<TournamentDto>>(cacheKey);
        if (cached != null) return Ok(cached);

        var tournaments = await _tournamentService.GetAllTournamentsAsync(page, pageSize, status, sport, level);
        
        await _redisService.SetDataAsync(cacheKey, tournaments, TimeSpan.FromMinutes(30), "tournaments_group");
        return Ok(tournaments);
    }



    [HttpGet("{id}")]
    public async Task<ActionResult<TournamentDto>> GetTournamentById(long id)
    {
        var cacheKey = $"tournament_{id}";
        var cached = await _redisService.GetDataAsync<TournamentDto>(cacheKey);
        if (cached != null) return Ok(cached);

        var tournament = await _tournamentService.GetTournamentByIdAsync(id);
        if (tournament == null) return NotFound();

        await _redisService.SetDataAsync(cacheKey, tournament, TimeSpan.FromHours(24), "tournaments_byid_group");
        return Ok(tournament);
    }


    [HttpGet("stats")]
    public async Task<ActionResult<GeneralStatsDto>> GetStats()
    {
        var stats = await _tournamentService.GetGeneralStatsAsync();
        return Ok(stats);
    }

    [HttpGet("recent-results")]
    public async Task<ActionResult<IEnumerable<RecentMatchDto>>> GetRecentResults([FromQuery] int count = 4, [FromQuery] string? sport = null)
    {
        var results = await _tournamentService.GetRecentMatchesAsync(count, sport);
        return Ok(results);
    }

    [HttpGet("champions")]
    public async Task<ActionResult<IEnumerable<PolytechLeague.Application.Dto.General.ChampionDto>>> GetChampions([FromQuery] string? sport = null, [FromQuery] PolytechLeague.Domain.Entities.Enums.Gender? gender = null, [FromQuery] string? weightClass = null)
    {
        var champions = await _tournamentService.GetChampionsAsync(sport, gender, weightClass);
        return Ok(champions);
    }

    [HttpPost("{id}/vote")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<ActionResult> Vote(long id, [FromQuery] long? teamId, [FromQuery] long? playerId)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !long.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var result = await _tournamentService.VoteAsync(id, userId, teamId, playerId);
        if (!result) return BadRequest("Не вдалося проголосувати. Можливо, ви вже голосували або змагання вже розпочалися.");

        return Ok();
    }

    [HttpGet("{id}/votes")]
    public async Task<ActionResult<VoteStatsDto>> GetVotes(long id)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        long? userId = null;
        if (!string.IsNullOrEmpty(userIdStr) && long.TryParse(userIdStr, out var parsedUserId))
        {
            userId = parsedUserId;
        }

        var stats = await _tournamentService.GetVoteStatsAsync(id, userId);
        return Ok(stats);
    }
}
