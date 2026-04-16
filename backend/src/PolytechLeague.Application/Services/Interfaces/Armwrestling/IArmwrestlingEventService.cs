using PolytechLeague.Domain.Entities.Armwrestling;

namespace PolytechLeague.Application.Services.Interfaces.Armwrestling;

public interface IArmwrestlingEventService : IAbstractEntityService<ArmwrestlingEvent>
{
    Task AddParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken);
    Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken);
}
