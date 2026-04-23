using PolytechLeague.Domain.Entities.Armwrestling;
using PolytechLeague.Application.Dto.ArmwrestlingDto;

namespace PolytechLeague.Application.Services.Interfaces.Armwrestling;

public interface IArmwrestlingEventService : IAbstractEntityService<ArmwrestlingEvent>
{
    Task AddParticipantAsync(long eventId, ArmwrestlingEventParticipantDto dto, CancellationToken cancellationToken);
    Task UpdateParticipantAsync(long eventId, long playerId, ArmwrestlingEventParticipantDto dto, CancellationToken cancellationToken);
    Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken);
    Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task ApproveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task RejectParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
}
