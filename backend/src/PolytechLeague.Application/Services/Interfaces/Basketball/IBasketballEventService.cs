using PolytechLeague.Domain.Entities.Basketball;

namespace PolytechLeague.Application.Services.Interfaces.Basketball;

public interface IBasketballEventService : IAbstractEntityService<BasketballEvent>
{
    Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task ApproveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task RejectParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
}
