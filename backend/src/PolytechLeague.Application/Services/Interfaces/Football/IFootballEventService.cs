using PolytechLeague.Domain.Entities.Football;

namespace PolytechLeague.Application.Services.Interfaces.Football;

public interface IFootballEventService : IAbstractEntityService<FootballEvent>
{
    Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task ApproveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task RejectParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
}
