using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;

namespace PolytechLeague.API.Controllers.Common;

[ApiController]
[Route("api/[controller]")]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;

    public TournamentsController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<TournamentDto>>> GetAllTournaments(
        int page = 1, 
        int pageSize = 10,
        string? status = null,
        string? sport = null,
        string? level = null)
    {
        var tournaments = await _tournamentService.GetAllTournamentsAsync(page, pageSize, status, sport, level);
        return Ok(tournaments);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<TournamentDto>> GetTournamentById(long id)
    {
        var tournament = await _tournamentService.GetTournamentByIdAsync(id);
        if (tournament == null) return NotFound();
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
}
