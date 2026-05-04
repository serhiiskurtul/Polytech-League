using PolytechLeague.Domain.Entities.Athletics;
using PolytechLeague.Application.Dto.AthleticsDto;

namespace PolytechLeague.Application.Services.Interfaces.Athletics;

public interface IAthleticsEventService : IAbstractEntityService<AthleticsEvent>
{
    Task AddResultAsync(long eventId, AthleticsResultDto dto, CancellationToken cancellationToken = default);
    Task UpdateResultAsync(long eventId, long resultId, AthleticsResultDto dto, CancellationToken cancellationToken = default);
    Task RemoveResultAsync(long eventId, long resultId, CancellationToken cancellationToken = default);
    
    Task AddParticipantAsync(long eventId, AthleticsEventParticipantDto dto, CancellationToken cancellationToken = default);
    Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
}
