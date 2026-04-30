using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;

namespace PolytechLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeasonRatingController : ControllerBase
{
    private readonly ISeasonRatingService _seasonRatingService;

    public SeasonRatingController(ISeasonRatingService seasonRatingService)
    {
        _seasonRatingService = seasonRatingService;
    }

    [HttpGet("{seasonId}")]
    public async Task<ActionResult<SeasonRatingDto>> GetSeasonRating(long seasonId)
    {
        var rating = await _seasonRatingService.GetSeasonRatingAsync(seasonId);
        return Ok(rating);
    }

    [HttpGet("national/{seasonId}")]
    public async Task<ActionResult<SeasonRatingDto>> GetNationalRating(long seasonId)
    {
        var rating = await _seasonRatingService.GetNationalRatingAsync(seasonId);
        return Ok(rating);
    }

    [HttpGet("seasons")]
    public async Task<ActionResult<IEnumerable<SeasonDto>>> GetAllSeasons()
    {
        var seasons = await _seasonRatingService.GetAllSeasonsAsync();
        return Ok(seasons);
    }

    [HttpGet("current")]
    public async Task<ActionResult<SeasonDto>> GetCurrentSeason()
    {
        var season = await _seasonRatingService.GetCurrentSeasonAsync();
        if (season == null) return NotFound();
        return Ok(season);
    }

    [HttpPost("seasons")]
    public async Task<ActionResult<SeasonDto>> CreateSeason(SeasonDto dto)
    {
        var result = await _seasonRatingService.CreateSeasonAsync(dto);
        return Ok(result);
    }

    [HttpPut("seasons/{id}")]
    public async Task<ActionResult<SeasonDto>> UpdateSeason(long id, SeasonDto dto)
    {
        var result = await _seasonRatingService.UpdateSeasonAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("seasons/{id}")]
    public async Task<ActionResult> DeleteSeason(long id)
    {
        await _seasonRatingService.DeleteSeasonAsync(id);
        return NoContent();
    }
}
