using PolytechLeague.Domain.Entities.Futsal;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Futsal;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Services.Implementations.Futsal;

public class FutsalEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<FutsalEvent>> _logger,
    INotificationService _notificationService
) :
    AbstractEntityService<FutsalEvent>(_dbcontext, _logger, _notificationService),
    IFutsalEventService
{
    protected override void UpdateValues(FutsalEvent oldEntity, FutsalEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
        oldEntity.Rules = newEntity.Rules;
        oldEntity.Prizes = newEntity.Prizes;
        oldEntity.HeadRefereeName = newEntity.HeadRefereeName;
        oldEntity.Referees = newEntity.Referees;
        oldEntity.Schedule = newEntity.Schedule;
        oldEntity.FirstPlaceTeamId = newEntity.FirstPlaceTeamId;
        oldEntity.SecondPlaceTeamId = newEntity.SecondPlaceTeamId;
        oldEntity.ThirdPlaceTeamId = newEntity.ThirdPlaceTeamId;
    }

    public override async Task<FutsalEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.FutsalEvents
            .Include(e => e.Teams!)
                .ThenInclude(t => t.Players)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.HomeTeam)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.AwayTeam)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.HomeRoster)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.AwayRoster)
            .Include(e => e.Photos)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
        {
            throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(FutsalEvent), id);
        }

        return entity;
    }

    protected override async Task OnCreatedAsync(FutsalEvent entity, CancellationToken cancellationToken)
    {
        await _notificationService.SendNotificationToAllAsync(
            "Нове змагання!",
            $"Створено нове змагання з футзалу: {entity.Name}",
            NotificationType.Tournament,
            $"/tournaments/{entity.Id}"
        );
    }

    protected override async Task OnUpdatedAsync(FutsalEvent oldEntity, FutsalEvent newEntity, CancellationToken cancellationToken)
    {
        bool winnersSet = newEntity.FirstPlaceTeamId != null && newEntity.SecondPlaceTeamId != null && newEntity.ThirdPlaceTeamId != null;
        
        if (winnersSet)
        {
            await _notificationService.SendNotificationToAllAsync(
                "Змагання завершено!",
                $"Визначено переможців у змаганні з футзалу: {newEntity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{newEntity.Id}"
            );
        }
    }
}
