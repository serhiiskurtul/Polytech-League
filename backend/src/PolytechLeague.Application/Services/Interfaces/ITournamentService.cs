using PolytechLeague.Application.Dto;

namespace PolytechLeague.Application.Services.Interfaces;

public interface ITournamentService
{
    Task<IEnumerable<TournamentDto>> GetAllTournamentsAsync();
}
