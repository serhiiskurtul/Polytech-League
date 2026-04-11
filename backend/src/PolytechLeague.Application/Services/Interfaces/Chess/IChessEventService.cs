using PolytechLeague.Domain.Entities.Chess;

namespace PolytechLeague.Application.Services.Interfaces.Chess;

public interface IChessEventService : IAbstractEntityService<ChessEvent>
{
    Task AddParticipantAsync(long eventId, PolytechLeague.Application.Dto.ChessDto.ChessEventParticipantDto dto, CancellationToken cancellationToken = default);
    Task UpdateParticipantAsync(long eventId, long playerId, PolytechLeague.Application.Dto.ChessDto.ChessEventParticipantDto dto, CancellationToken cancellationToken = default);
    Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
}
