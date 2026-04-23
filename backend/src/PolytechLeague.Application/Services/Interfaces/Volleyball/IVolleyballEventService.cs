using PolytechLeague.Domain.Entities.Volleyball;

namespace PolytechLeague.Application.Services.Interfaces.Volleyball;

public interface IVolleyballEventService : IAbstractEntityService<VolleyballEvent>
{
    Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
}
