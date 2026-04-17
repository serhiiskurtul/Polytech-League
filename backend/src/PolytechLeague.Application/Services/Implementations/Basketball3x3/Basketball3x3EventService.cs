using PolytechLeague.Domain.Entities.Basketball3x3;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Basketball3x3;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Services.Implementations.Basketball3x3;

public class Basketball3x3EventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<Basketball3x3Event>> _logger,
    INotificationService _notificationService
) :
    AbstractEntityService<Basketball3x3Event>(_dbcontext, _logger, _notificationService),
    IBasketball3x3EventService
{
    protected override void UpdateValues(Basketball3x3Event oldEntity, Basketball3x3Event newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.Level = newEntity.Level;
        oldEntity.TournamentType = newEntity.TournamentType;
        oldEntity.Rules = newEntity.Rules;
        oldEntity.Prizes = newEntity.Prizes;
        oldEntity.HeadRefereeName = newEntity.HeadRefereeName;
        oldEntity.Referees = newEntity.Referees;
        oldEntity.Schedule = newEntity.Schedule;
        oldEntity.FirstPlaceTeamId = newEntity.FirstPlaceTeamId;
        oldEntity.SecondPlaceTeamId = newEntity.SecondPlaceTeamId;
        oldEntity.ThirdPlaceTeamId = newEntity.ThirdPlaceTeamId;
    }

    public override async Task<Basketball3x3Event> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.Basketball3x3Events
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
            throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(Basketball3x3Event), id);
        }

        return entity;
    }

    protected override async Task OnCreatedAsync(Basketball3x3Event entity, CancellationToken cancellationToken)
    {
        await _notificationService.SendNotificationToAllAsync(
            "Нове змагання!",
            $"Створено нове змагання з баскетболу 3х3: {entity.Name}",
            NotificationType.Tournament,
            $"/tournaments/{entity.Id}"
        );
    }

    protected override async Task OnUpdatedAsync(Basketball3x3Event oldEntity, Basketball3x3Event newEntity, CancellationToken cancellationToken)
    {
        bool winnersSet = newEntity.FirstPlaceTeamId != null && newEntity.SecondPlaceTeamId != null && newEntity.ThirdPlaceTeamId != null;
        
        if (winnersSet)
        {
            await _notificationService.SendNotificationToAllAsync(
                "Змагання завершено!",
                $"Визначено переможців у змаганні з баскетболу 3х3: {newEntity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{newEntity.Id}"
            );
        }
    }
}
