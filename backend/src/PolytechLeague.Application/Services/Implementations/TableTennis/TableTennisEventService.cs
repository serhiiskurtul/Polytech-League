using PolytechLeague.Domain.Entities.TableTennis;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Persistence.Database;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Application.Dto.TableTennisDto;

namespace PolytechLeague.Application.Services.Implementations.TableTennis;

public class TableTennisEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<TableTennisEvent>> _logger,
    INotificationService _notificationService
) :
    AbstractEntityService<TableTennisEvent>(_dbcontext, _logger, _notificationService),
    ITableTennisEventService
{
    protected override void UpdateValues(TableTennisEvent oldEntity, TableTennisEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
        oldEntity.TournamentType = newEntity.TournamentType;
        oldEntity.Category = newEntity.Category;
        oldEntity.FirstPlaceId = newEntity.FirstPlaceId;
        oldEntity.SecondPlaceId = newEntity.SecondPlaceId;
        oldEntity.ThirdPlaceId = newEntity.ThirdPlaceId;
        oldEntity.UpdatedAt = DateTime.UtcNow;
    }

    public override async Task<TableTennisEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbcontext.TableTennisEvents
            .Include(e => e.Photos)
            .Include(e => e.FirstPlace)
            .Include(e => e.SecondPlace)
            .Include(e => e.ThirdPlace)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.Player1)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.Player2)
            .Include(e => e.Participants!)
                .ThenInclude(p => p.Player)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(TableTennisEvent), id);
    }

    protected override async Task OnCreatedAsync(TableTennisEvent entity, CancellationToken cancellationToken)
    {
        await _notificationService.SendNotificationToAllAsync(
            "Нове змагання!",
            $"Створено нове змагання з настільного тенісу: {entity.Name}",
            NotificationType.Tournament,
            $"/tournaments/{entity.Id}"
        );
    }

    protected override async Task OnUpdatedAsync(TableTennisEvent oldEntity, TableTennisEvent newEntity, CancellationToken cancellationToken)
    {
        bool winnersSet = newEntity.FirstPlaceId != null && newEntity.SecondPlaceId != null && newEntity.ThirdPlaceId != null;
        
        if (winnersSet)
        {
            await _notificationService.SendNotificationToAllAsync(
                "Змагання завершено!",
                $"Визначено переможців у змаганні з настільного тенісу: {newEntity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{newEntity.Id}"
            );
        }
    }

    public async Task AddParticipantAsync(long eventId, TableTennisEventParticipantDto dto, CancellationToken cancellationToken = default)
    {
        var exists = await _dbcontext.Set<TableTennisEventParticipant>()
            .AnyAsync(p => p.TournamentId == eventId && p.PlayerId == dto.PlayerId, cancellationToken);

        if (exists) return;

        var participant = new TableTennisEventParticipant
        {
            TournamentId = eventId,
            PlayerId = dto.PlayerId,
            Points = dto.Points,
            Rank = dto.Rank
        };
        _dbcontext.Set<TableTennisEventParticipant>().Add(participant);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateParticipantAsync(long eventId, long playerId, TableTennisEventParticipantDto dto, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<TableTennisEventParticipant>()
            .FirstOrDefaultAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);
        if (participant != null)
        {
            participant.Points = dto.Points;
            participant.Rank = dto.Rank;
            _dbcontext.Set<TableTennisEventParticipant>().Update(participant);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<TableTennisEventParticipant>()
            .FirstOrDefaultAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);
        if (participant != null)
        {
            _dbcontext.Set<TableTennisEventParticipant>().Remove(participant);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }
    }
}
