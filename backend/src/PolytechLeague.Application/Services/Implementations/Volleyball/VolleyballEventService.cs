using PolytechLeague.Domain.Entities.Volleyball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Volleyball;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Exceptions;
using PolytechLeague.Domain.Entities.Shared;
using PolytechLeague.Domain.Entities.Enums;

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
        oldEntity.IsModerated = newEntity.IsModerated;
        oldEntity.IsPublic = newEntity.IsPublic;
        oldEntity.IsTeam = newEntity.IsTeam;
        oldEntity.SeasonId = newEntity.SeasonId;
        oldEntity.MvpId = newEntity.MvpId;
        oldEntity.Category = newEntity.Category;

    }

    public override async Task<VolleyballEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.VolleyballEvents
            .Include(e => e.Participants!)
                .ThenInclude(p => p.Player)
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
            .Include(e => e.FirstPlaceTeam)
            .Include(e => e.SecondPlaceTeam)
            .Include(e => e.ThirdPlaceTeam)
            .Include(e => e.Mvp)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);


        if (entity is null)
        {
            throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(VolleyballEvent), id);
        }

        return entity;
    }

    protected override async Task OnCreatedAsync(VolleyballEvent entity, CancellationToken cancellationToken)
    {
        if (_notificationService != null)
        {
            await _notificationService.SendNotificationToAllAsync(
                "Нове змагання!",
                $"Створено нове змагання з волейболу: {entity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{entity.Id}",
                true
            );
        }
    }

    protected override async Task OnUpdatedAsync(VolleyballEvent oldEntity, VolleyballEvent newEntity, CancellationToken cancellationToken)
    {
        // Update ranks for teams based on winners
        var eventWithTeams = await _dbcontext.VolleyballEvents
            .Include(e => e.Teams)
            .FirstOrDefaultAsync(e => e.Id == oldEntity.Id, cancellationToken);

        if (eventWithTeams != null && eventWithTeams.Teams != null)
        {
            foreach (var team in eventWithTeams.Teams)
            {
                if (team.Id == oldEntity.FirstPlaceTeamId) team.Rank = 1;
                else if (team.Id == oldEntity.SecondPlaceTeamId) team.Rank = 2;
                else if (team.Id == oldEntity.ThirdPlaceTeamId) team.Rank = 3;
                else if (oldEntity.FirstPlaceTeamId != null) team.Rank = 4;
                else team.Rank = 0;
            }
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }

        bool winnersSet = oldEntity.FirstPlaceTeamId != null && oldEntity.SecondPlaceTeamId != null && oldEntity.ThirdPlaceTeamId != null;
        
        if (winnersSet)
        {
            if (_notificationService != null)
            {
                await _notificationService.SendNotificationToAllAsync(
                    "Турнір завершено!",
                    $"Змагання з волейболу \"{oldEntity.Name}\" завершено. Перегляньте результати!",
                    NotificationType.Tournament,
                    $"/tournaments/{oldEntity.Id}",
                    true
                );
            }
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

        var player = await _dbcontext.Set<Player>()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == playerId, cancellationToken)
            ?? throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(Player), playerId);

        if (player.User?.Role == UserRole.Admin)
        {
            throw new InvalidOperationException("Administrators cannot register for tournaments.");
        }

        if (player.Group == "Гість")
        {
            throw new InvalidOperationException("Користувачі з групою 'Гість' не можуть реєструватися на змагання. Будь ласка, оновіть свою групу в профілі.");
        }

        if (string.IsNullOrWhiteSpace(player.StudentId))
        {
            throw new InvalidOperationException("Для реєстрації на змагання необхідно вказати серію та номер студентського квитка в профілі (наприклад: PH12345678).");
        }

        if (player.Gender == Gender.None)
        {
            throw new InvalidOperationException("Для реєстрації на змагання необхідно вказати стать у профілі.");
        }

        var groupName = player.Group;
        if (string.IsNullOrWhiteSpace(groupName))
        {
            throw new InvalidOperationException("Player must have a group to register for inter-group competitions.");
        }

        tournament.Teams ??= new List<VolleyballTeam>();
        tournament.Participants ??= new List<VolleyballEventParticipant>();

        // Check if player is already registered as a participant
        var existingParticipant = tournament.Participants.FirstOrDefault(p => p.PlayerId == playerId);
        if (existingParticipant != null) return;

        var status = tournament.IsModerated ? RegistrationStatus.Pending : RegistrationStatus.Approved;

        var participant = new VolleyballEventParticipant
        {
            EventId = eventId,
            PlayerId = playerId,
            Status = status,
            GroupName = player.Group
        };

        tournament.Participants.Add(participant);

        if (status == RegistrationStatus.Approved)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
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
                if (!team.Players.Any(p => p.Id == playerId))
                {
                    team.Players.Add(player);
                }
            }
        }

        await _dbcontext.SaveChangesAsync(cancellationToken);
        
        if (status == RegistrationStatus.Pending)
        {
            await _notificationService.SendNotificationToAdminsAsync(
                "Новий запит на реєстрацію (волейбол)",
                $"Гравець {player.FirstName} {player.LastName} (група {player.Group}) хоче зареєструватися на {tournament.Name}",
                NotificationType.Tournament,
                $"/tournaments/{eventId}"
            );
        }
        else
        {
            await _notificationService.SendNotificationToAdminsAsync(
                "Нова реєстрація на волейбол",
                $"Гравець {player.FirstName} {player.LastName} (група {player.Group}) зареєструвався на {tournament.Name}",
                NotificationType.Tournament,
                $"/tournaments/{eventId}"
            );
        }
    }

    public async Task ApproveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var tournament = await _dbcontext.VolleyballEvents
            .Include(e => e.Participants!)
            .Include(e => e.Teams!)
            .ThenInclude(t => t.Players)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

        if (tournament == null) return;

        var participant = tournament.Participants?.FirstOrDefault(p => p.PlayerId == playerId);
        if (participant == null || participant.Status == RegistrationStatus.Approved) return;

        participant.Status = RegistrationStatus.Approved;

        var player = await _dbcontext.Players.FindAsync(new object[] { playerId }, cancellationToken);
        if (player != null)
        {
            var groupName = participant.GroupName ?? player.Group;
            if (!string.IsNullOrEmpty(groupName))
            {
                var team = tournament.Teams?.FirstOrDefault(t => t.Name == groupName);
                if (team == null)
                {
                    team = new VolleyballTeam
                    {
                        Name = groupName,
                        EventId = eventId,
                        Players = new List<Player>()
                    };
                    tournament.Teams!.Add(team);
                }

                team.Players ??= new List<Player>();
                if (!team.Players.Any(p => p.Id == playerId))
                {
                    team.Players.Add(player);
                }
            }

            if (player.UserId.HasValue)
            {
                await _notificationService.SendNotificationToUserAsync(
                    player.UserId.Value,
                    "Реєстрація схвалена",
                    $"Вашу заявку на участь у змаганні з волейболу \"{tournament.Name}\" схвалено!",
                    NotificationType.Tournament,
                    $"/tournaments/{eventId}"
                );
            }
        }

        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.VolleyballEventParticipants
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.PlayerId == playerId, cancellationToken);

        var tournament = await _dbcontext.VolleyballEvents.FindAsync(eventId);

        if (participant != null)
        {
            participant.Status = RegistrationStatus.Rejected;
            await _dbcontext.SaveChangesAsync(cancellationToken);

            var player = await _dbcontext.Players.FindAsync(new object[] { playerId }, cancellationToken);
            if (player?.UserId.HasValue == true)
            {
                await _notificationService.SendNotificationToUserAsync(
                    player.UserId.Value,
                    "Реєстрація відхилена",
                    $"Вашу заявку на участь у змаганні з волейболу \"{tournament?.Name}\" було відхилено.",
                    NotificationType.Tournament,
                    $"/tournaments/{eventId}"
                );
            }
        }
    }

    public async Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.VolleyballEventParticipants
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.PlayerId == playerId, cancellationToken);

        if (participant != null)
        {
            _dbcontext.VolleyballEventParticipants.Remove(participant);

            // Also remove from team if exists
            var teamWithPlayer = await _dbcontext.VolleyballTeams
                .Include(t => t.Players)
                .FirstOrDefaultAsync(t => t.EventId == eventId && t.Players!.Any(p => p.Id == playerId), cancellationToken);

            if (teamWithPlayer != null)
            {
                var p = teamWithPlayer.Players!.First(p => p.Id == playerId);
                teamWithPlayer.Players!.Remove(p);
            }

            await _dbcontext.SaveChangesAsync(cancellationToken);
        }
    }

    public override async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        // 0. Clear player references
        await _dbcontext.Database.ExecuteSqlRawAsync(
            $"UPDATE players SET volleyball_team_id = NULL WHERE volleyball_team_id IN (SELECT \"Id\" FROM volleyball_teams WHERE \"EventId\" = {id})", 
            cancellationToken);

        // 1. Fetch event
        var entity = await _dbcontext.VolleyballEvents
            .Include(e => e.Matches)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity == null) throw new EntityNotFoundException(nameof(VolleyballEvent), id);

        // 2. Clear winners
        entity.FirstPlaceTeamId = null;
        entity.SecondPlaceTeamId = null;
        entity.ThirdPlaceTeamId = null;
        await _dbcontext.SaveChangesAsync(cancellationToken);

        // 3. Delete matches
        if (entity.Matches != null && entity.Matches.Any())
        {
            _dbcontext.VolleyballMatches.RemoveRange(entity.Matches);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }

        // 4. Delete event
        _dbcontext.VolleyballEvents.Remove(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }
}
