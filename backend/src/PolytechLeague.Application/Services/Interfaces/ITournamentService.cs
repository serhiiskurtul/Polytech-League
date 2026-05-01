using PolytechLeague.Application.Dto;

namespace PolytechLeague.Application.Services.Interfaces;

public interface ITournamentService
{
    Task<PagedResponse<TournamentDto>> GetAllTournamentsAsync(int page = 1, int pageSize = 10, string? status = null, string? sport = null, string? level = null);

    Task<TournamentDto?> GetTournamentByIdAsync(long id);
    Task<GeneralStatsDto> GetGeneralStatsAsync();
    Task<IEnumerable<RecentMatchDto>> GetRecentMatchesAsync(int count, string? sport = null);
    Task<IEnumerable<PolytechLeague.Application.Dto.General.ChampionDto>> GetChampionsAsync(string? sportType = null);
}
