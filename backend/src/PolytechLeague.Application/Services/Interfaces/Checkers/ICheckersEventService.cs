using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Application.Dto.CheckersDto;

namespace PolytechLeague.Application.Services.Interfaces.Checkers;

public interface ICheckersEventService : IAbstractEntityService<CheckersEvent>
{
    Task AddParticipantAsync(long id, CheckersEventParticipantDto dto, CancellationToken cancellationToken = default);
    Task UpdateParticipantAsync(long id, long playerId, CheckersEventParticipantDto dto, CancellationToken cancellationToken = default);
    Task RemoveParticipantAsync(long id, long playerId, CancellationToken cancellationToken = default);
    Task AddPhotoAsync(long id, string url, CancellationToken cancellationToken = default);
    Task RemovePhotoAsync(long id, long photoId, CancellationToken cancellationToken = default);
    Task ImportDataFromUrlAsync(long eventId, string url, CancellationToken cancellationToken = default);
    Task ImportDataFromFileAsync(long eventId, Stream stream, CancellationToken cancellationToken = default);
}
