using PolytechLeague.Domain.Entities.Futsal;

namespace PolytechLeague.Application.Services.Interfaces.Futsal;

public interface IFutsalEventService : IAbstractEntityService<FutsalEvent>
{
    Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
}
