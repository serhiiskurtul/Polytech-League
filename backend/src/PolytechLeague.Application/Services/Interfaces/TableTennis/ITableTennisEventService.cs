using PolytechLeague.Domain.Entities.TableTennis;
using PolytechLeague.Application.Dto.TableTennisDto;

namespace PolytechLeague.Application.Services.Interfaces.TableTennis;

public interface ITableTennisEventService : IAbstractEntityService<TableTennisEvent>
{
    Task AddParticipantAsync(long eventId, TableTennisEventParticipantDto dto, CancellationToken cancellationToken = default);
    Task UpdateParticipantAsync(long eventId, long playerId, TableTennisEventParticipantDto dto, CancellationToken cancellationToken = default);
    Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task ApproveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
    Task RejectParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default);
}
