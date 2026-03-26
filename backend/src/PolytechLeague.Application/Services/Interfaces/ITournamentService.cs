using PolytechLeague.Application.Dto;

namespace PolytechLeague.Application.Services.Interfaces;

public interface ITournamentService
{
    Task<PagedResponse<TournamentDto>> GetAllTournamentsAsync(int page = 1, int pageSize = 10);
}
