using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using PolytechLeague.Application.Services.Interfaces;

namespace PolytechLeague.API.Controllers.Common;

/// <summary>
/// Відкрите API для інтеграції
/// </summary>
[ApiController]
[Route("api/public")]
[EnableCors("PublicApi")]
public class PublicApiController : ControllerBase
{
    private readonly ITournamentService _tournamentService;

    public PublicApiController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    /// <summary>
    /// Отримати список усіх спортивних подій.
    /// </summary>
    /// <param name="sport">Опціональний фільтр за видом спорту (напр. 'football', 'volleyball')</param>
    /// <returns>Список подій</returns>
    [HttpGet("events")]
    public async Task<IActionResult> GetEvents([FromQuery] string? sport = null)
    {
        // Отримуємо до 100 подій для публічного доступу
        var events = await _tournamentService.GetAllTournamentsAsync(1, 100, "all", sport, "all");
        return Ok(events.Items);
    }

    /// <summary>
    /// Отримати детальну інформацію про конкретну спортивну подію.
    /// </summary>
    /// <param name="id">ID події</param>
    /// <returns>Деталі події</returns>
    [HttpGet("events/{id}")]
    public async Task<IActionResult> GetEventDetails(long id)
    {
        var tournament = await _tournamentService.GetTournamentByIdAsync(id);
        if (tournament == null) return NotFound();
        return Ok(tournament);
    }
}
