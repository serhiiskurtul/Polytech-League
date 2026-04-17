using PolytechLeague.Domain.Entities.Badminton;
using PolytechLeague.Application.Dto.BadmintonDto;

namespace PolytechLeague.Application.Services.Interfaces.Badminton;

public interface IBadmintonEventService : IAbstractEntityService<BadmintonEvent>
{
    Task AddParticipantAsync(long eventId, BadmintonEventParticipantDto dto, CancellationToken cancellationToken = default);
    Task UpdateParticipantAsync(long eventId, long playerId, BadmintonEventParticipantDto dto, CancellationToken cancellationToken = default);
    Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
}

public interface IBadmintonMatchService : IAbstractEntityService<BadmintonMatch> { }
public interface IBadmintonParticipantService : IAbstractEntityService<BadmintonEventParticipant> { }
