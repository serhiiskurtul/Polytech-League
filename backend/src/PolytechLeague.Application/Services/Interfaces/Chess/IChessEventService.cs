using PolytechLeague.Domain.Entities.Chess;

namespace PolytechLeague.Application.Services.Interfaces.Chess;

public interface IChessEventService : IAbstractEntityService<ChessEvent>
{
    Task AddParticipantAsync(long eventId, PolytechLeague.Application.Dto.ChessDto.ChessEventParticipantDto dto, CancellationToken cancellationToken = default);
    Task UpdateParticipantAsync(long eventId, long playerId, PolytechLeague.Application.Dto.ChessDto.ChessEventParticipantDto dto, CancellationToken cancellationToken = default);
    Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task AddPhotoAsync(long eventId, string url, CancellationToken cancellationToken = default);
    Task RemovePhotoAsync(long eventId, long photoId, CancellationToken cancellationToken = default);
    Task ImportDataFromUrlAsync(long eventId, string url, CancellationToken cancellationToken = default);
    Task ImportDataFromFileAsync(long eventId, Stream stream, CancellationToken cancellationToken = default);
    Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task ApproveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task RejectParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
}
