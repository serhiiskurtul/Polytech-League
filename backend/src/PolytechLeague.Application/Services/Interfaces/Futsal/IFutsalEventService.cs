using PolytechLeague.Domain.Entities.Futsal;

namespace PolytechLeague.Application.Services.Interfaces.Futsal;

public interface IFutsalEventService : IAbstractEntityService<FutsalEvent>
{
    Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task ApproveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task RejectParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
}
