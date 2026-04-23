using PolytechLeague.Domain.Entities.BeachVolleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.BeachVolleyball;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Services.Implementations.BeachVolleyball;

public class BeachVolleyballEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BeachVolleyballEvent>> _logger,
    INotificationService _notificationService
) :
    AbstractEntityService<BeachVolleyballEvent>(_dbcontext, _logger, _notificationService),
    IBeachVolleyballEventService
{
    protected override void UpdateValues(BeachVolleyballEvent oldEntity, BeachVolleyballEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.RegistrationStart = newEntity.RegistrationStart;
        oldEntity.RegistrationEnd = newEntity.RegistrationEnd;
        oldEntity.EndDate = newEntity.EndDate;
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

    public override async Task<BeachVolleyballEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.BeachVolleyballEvents
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
            throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(BeachVolleyballEvent), id);
        }

        return entity;
    }

    protected override async Task OnCreatedAsync(BeachVolleyballEvent entity, CancellationToken cancellationToken)
    {
        await _notificationService.SendNotificationToAllAsync(
            "Нове змагання!",
            $"Створено нове змагання з пляжного волейболу: {entity.Name}",
            NotificationType.Tournament,
            $"/tournaments/{entity.Id}"
        );
    }

    protected override async Task OnUpdatedAsync(BeachVolleyballEvent oldEntity, BeachVolleyballEvent newEntity, CancellationToken cancellationToken)
    {
        bool winnersSet = newEntity.FirstPlaceTeamId != null && newEntity.SecondPlaceTeamId != null && newEntity.ThirdPlaceTeamId != null;
        
        if (winnersSet)
        {
            await _notificationService.SendNotificationToAllAsync(
                "Змагання завершено!",
                $"Визначено переможців у змаганні з пляжного волейболу: {newEntity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{newEntity.Id}"
            );
        }
    }
}
