using PolytechLeague.Domain.Entities.BeachVolleyball;

namespace PolytechLeague.Application.Services.Interfaces.BeachVolleyball;

public interface IBeachVolleyballEventService : IAbstractEntityService<BeachVolleyballEvent>
{
    Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
}
