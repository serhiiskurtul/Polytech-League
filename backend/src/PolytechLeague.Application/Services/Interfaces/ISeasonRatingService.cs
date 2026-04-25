using PolytechLeague.Application.Dto;

namespace PolytechLeague.Application.Services.Interfaces;

public interface ISeasonRatingService
{
    Task<SeasonRatingDto> GetSeasonRatingAsync(long seasonId);
    Task<IEnumerable<SeasonDto>> GetAllSeasonsAsync();
    Task<SeasonDto?> GetCurrentSeasonAsync();
    
    // Admin methods
    Task<SeasonDto> CreateSeasonAsync(SeasonDto seasonDto);
    Task<SeasonDto> UpdateSeasonAsync(long id, SeasonDto seasonDto);
    Task DeleteSeasonAsync(long id);
}
