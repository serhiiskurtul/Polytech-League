using PolytechLeague.Domain.Entities.Basketball3x3;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Basketball3x3;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Exceptions;

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
        oldEntity.RegistrationStart = newEntity.RegistrationStart;
        oldEntity.RegistrationEnd = newEntity.RegistrationEnd;
        oldEntity.EndDate = newEntity.EndDate;
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

    public async Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var tournament = await _dbcontext.Basketball3x3Events
            .Include(e => e.Teams!)
            .ThenInclude(t => t.Players)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

        if (tournament == null) throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(Basketball3x3Event), eventId);

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

        tournament.Teams ??= new List<Basketball3x3Team>();

        // Check if player is already in any team for this event
        var alreadyRegistered = tournament.Teams.Any(t => t.Players?.Any(p => p.Id == playerId) == true);
        if (alreadyRegistered) return;

        var team = tournament.Teams.FirstOrDefault(t => t.Name == groupName);
        if (team == null)
        {
            team = new Basketball3x3Team
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
            "Нова реєстрація на баскетбол 3х3",
            $"Гравець {player.FirstName} {player.LastName} (група {groupName}) зареєструвався на {tournament.Name}",
            NotificationType.Tournament,
            $"/tournaments/{eventId}"
        );
    }

    public override async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        // 0. Clear player references
        await _dbcontext.Database.ExecuteSqlRawAsync(
            $"UPDATE players SET basketball_team_id = NULL WHERE basketball_team_id IN (SELECT id FROM basketball3x3_teams WHERE \"EventId\" = {id})", 
            cancellationToken);

        // 1. Fetch event
        var entity = await _dbcontext.Basketball3x3Events
            .Include(e => e.Matches)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity == null) throw new EntityNotFoundException(nameof(Basketball3x3Event), id);

        // 2. Clear winners
        entity.FirstPlaceTeamId = null;
        entity.SecondPlaceTeamId = null;
        entity.ThirdPlaceTeamId = null;
        await _dbcontext.SaveChangesAsync(cancellationToken);

        // 3. Delete matches
        if (entity.Matches != null && entity.Matches.Any())
        {
            _dbcontext.Basketball3x3Matches.RemoveRange(entity.Matches);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }

        // 4. Delete event
        _dbcontext.Basketball3x3Events.Remove(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }
}
