using PolytechLeague.Domain.Entities.Volleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Volleyball;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Services.Implementations.Volleyball;

public class VolleyballEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<VolleyballEvent>> _logger,
    INotificationService _notificationService
) :
    AbstractEntityService<VolleyballEvent>(_dbcontext, _logger, _notificationService),
    IVolleyballEventService
{
    protected override void UpdateValues(VolleyballEvent oldEntity, VolleyballEvent newEntity)
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

    public override async Task<VolleyballEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.VolleyballEvents
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
            throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(VolleyballEvent), id);
        }

        return entity;
    }

    protected override async Task OnCreatedAsync(VolleyballEvent entity, CancellationToken cancellationToken)
    {
        await _notificationService.SendNotificationToAllAsync(
            "Нове змагання!",
            $"Створено нове змагання з волейболу: {entity.Name}",
            NotificationType.Tournament,
            $"/tournaments/{entity.Id}"
        );
    }

    protected override async Task OnUpdatedAsync(VolleyballEvent oldEntity, VolleyballEvent newEntity, CancellationToken cancellationToken)
    {
        bool winnersSet = newEntity.FirstPlaceTeamId != null && newEntity.SecondPlaceTeamId != null && newEntity.ThirdPlaceTeamId != null;
        
        if (winnersSet)
        {
            await _notificationService.SendNotificationToAllAsync(
                "Змагання завершено!",
                $"Визначено переможців у змаганні з волейболу: {newEntity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{newEntity.Id}"
            );
        }
    }

    public async Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var tournament = await _dbcontext.VolleyballEvents
            .Include(e => e.Teams!)
            .ThenInclude(t => t.Players)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

        if (tournament == null) throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(VolleyballEvent), eventId);

        if (tournament.Level != CompetitionLevel.CollegeGroup)
        {
            throw new InvalidOperationException("Self-registration is only allowed for college group tournaments.");
        }

        var player = await _dbcontext.Players.FindAsync([playerId], cancellationToken);
        if (player == null) throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(Player), playerId);

        var groupName = player.Group;
        if (string.IsNullOrWhiteSpace(groupName))
        {
            throw new InvalidOperationException("Player must have a group to register for inter-group competitions.");
        }

        tournament.Teams ??= new List<VolleyballTeam>();

        // Check if player is already in any team for this event
        var alreadyRegistered = tournament.Teams.Any(t => t.Players?.Any(p => p.Id == playerId) == true);
        if (alreadyRegistered) return;

        var team = tournament.Teams.FirstOrDefault(t => t.Name == groupName);
        if (team == null)
        {
            team = new VolleyballTeam
            {
                Name = groupName,
                EventId = eventId,
                Players = new List<Player>()
            };
            tournament.Teams.Add(team);
        }

        team.Players ??= new List<Player>();
        team.Players.Add(player);

        await _dbcontext.SaveChangesAsync(cancellationToken);
        
        await _notificationService.SendNotificationToAdminsAsync(
            "Нова реєстрація на волейбол",
            $"Гравець {player.FirstName} {player.LastName} (група {groupName}) зареєструвався на {tournament.Name}",
            NotificationType.Tournament,
            $"/tournaments/{eventId}"
        );
    }
}
