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
        string? sport = null)
    {
        var tournaments = await _tournamentService.GetAllTournamentsAsync(page, pageSize, status, sport);
        return Ok(tournaments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TournamentDto>> GetTournamentById(long id)
    {
        var tournament = await _tournamentService.GetTournamentByIdAsync(id);
        if (tournament == null) return NotFound();
        return Ok(tournament);
    }
}
