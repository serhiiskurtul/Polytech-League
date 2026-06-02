using PolytechLeague.Domain.Entities.Basketball;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Basketball;
using PolytechLeague.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Exceptions;
using PolytechLeague.Domain.Entities.Shared;
using PolytechLeague.Domain.Entities.Enums;

namespace PolytechLeague.Application.Services.Implementations.Basketball;

public class BasketballEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<BasketballEvent>> _logger,
    INotificationService _notificationService
) :
    AbstractEntityService<BasketballEvent>(_dbcontext, _logger, _notificationService),
    IBasketballEventService
{
    protected override void UpdateValues(BasketballEvent updatedEntity, BasketballEvent newEntity)
    {
        updatedEntity.Name = newEntity.Name;
        updatedEntity.Date = newEntity.Date;
        updatedEntity.Location = newEntity.Location;
        updatedEntity.Description = newEntity.Description;
        updatedEntity.RegistrationStart = newEntity.RegistrationStart;
        updatedEntity.RegistrationEnd = newEntity.RegistrationEnd;
        updatedEntity.EndDate = newEntity.EndDate;
        updatedEntity.Level = newEntity.Level;
        updatedEntity.TournamentType = newEntity.TournamentType;
        updatedEntity.Rules = newEntity.Rules;
        updatedEntity.Prizes = newEntity.Prizes;
        updatedEntity.HeadRefereeName = newEntity.HeadRefereeName;
        updatedEntity.Referees = newEntity.Referees;
        updatedEntity.Schedule = newEntity.Schedule;
        updatedEntity.FirstPlaceTeamId = newEntity.FirstPlaceTeamId;
        updatedEntity.SecondPlaceTeamId = newEntity.SecondPlaceTeamId;
        updatedEntity.ThirdPlaceTeamId = newEntity.ThirdPlaceTeamId;
        updatedEntity.IsModerated = newEntity.IsModerated;
        updatedEntity.IsPublic = newEntity.IsPublic;
        updatedEntity.DisableNotifications = newEntity.DisableNotifications;
        updatedEntity.IsTeam = newEntity.IsTeam;
        updatedEntity.SeasonId = newEntity.SeasonId;
        updatedEntity.MvpId = newEntity.MvpId;
        updatedEntity.Category = newEntity.Category;

    }

    public override async Task<BasketballEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbcontext.BasketballEvents
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
            throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(BasketballEvent), id);
        }

        return entity;
    }

    protected override async Task OnCreatedAsync(BasketballEvent entity, CancellationToken cancellationToken)
    {
        if (_notificationService != null)
        {
            if (!entity.DisableNotifications)
        {
            await _notificationService!.SendNotificationToAllAsync(
                "Нове змагання!",
                $"Створено нове змагання з баскетболу 3х3: {entity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{entity.Id}",
                true
            );
        }
        }
    }

    protected override async Task OnUpdatedAsync(BasketballEvent originalEntity, BasketballEvent updatedEntity, CancellationToken cancellationToken)
    {
        // Update ranks for teams based on winners
        var eventWithTeams = await _dbcontext.BasketballEvents
            .Include(e => e.Teams)
            .FirstOrDefaultAsync(e => e.Id == updatedEntity.Id, cancellationToken);

        if (eventWithTeams != null && eventWithTeams.Teams != null)
        {
            foreach (var team in eventWithTeams.Teams)
            {
                if (team.Id == updatedEntity.FirstPlaceTeamId) team.Rank = 1;
                else if (team.Id == updatedEntity.SecondPlaceTeamId) team.Rank = 2;
                else if (team.Id == updatedEntity.ThirdPlaceTeamId) team.Rank = 3;
                else if (updatedEntity.FirstPlaceTeamId != null) team.Rank = 4; // Participation rank
                else team.Rank = 0;
            }
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }

        bool winnersSet = updatedEntity.FirstPlaceTeamId != null && updatedEntity.SecondPlaceTeamId != null && updatedEntity.ThirdPlaceTeamId != null && (originalEntity.FirstPlaceTeamId == null || originalEntity.SecondPlaceTeamId == null || originalEntity.ThirdPlaceTeamId == null);
        
        if (winnersSet && !updatedEntity.DisableNotifications && _notificationService != null)
        {
            if (!updatedEntity.DisableNotifications)
        {
            await _notificationService!.SendNotificationToAllAsync(
                "Турнір завершено!",
                $"Змагання з баскетболу 3х3 \"{updatedEntity.Name}\" завершено. Перегляньте результати!",
                NotificationType.Tournament,
                $"/tournaments/{updatedEntity.Id}",
                true
            );
        }
        }
    }

    public async Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var tournament = await _dbcontext.BasketballEvents
            .Include(e => e.Teams!)
            .ThenInclude(t => t.Players)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

        if (tournament == null) throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(BasketballEvent), eventId);

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

        tournament.Teams ??= new List<BasketballTeam>();
        tournament.Participants ??= new List<BasketballEventParticipant>();

        // Check if player is already registered as a participant
        var existingParticipant = tournament.Participants.FirstOrDefault(p => p.PlayerId == playerId);
        if (existingParticipant != null) return;

        var status = tournament.IsModerated ? RegistrationStatus.Pending : RegistrationStatus.Approved;

        var participant = new BasketballEventParticipant
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
                    team = new BasketballTeam
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
                "Новий запит на реєстрацію (баскетбол 3х3)",
                $"Гравець {player.FirstName} {player.LastName} (група {player.Group}) хоче зареєструватися на {tournament.Name}",
                NotificationType.Tournament,
                $"/tournaments/{eventId}"
            );
        }
        else
        {
            await _notificationService.SendNotificationToAdminsAsync(
                "Нова реєстрація на баскетбол 3х3",
                $"Гравець {player.FirstName} {player.LastName} (група {player.Group}) зареєструвався на {tournament.Name}",
                NotificationType.Tournament,
                $"/tournaments/{eventId}"
            );
        }
    }

    public async Task ApproveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var tournament = await _dbcontext.BasketballEvents
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
                    team = new BasketballTeam
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
                    $"Вашу заявку на участь у змаганні з баскетболу 3х3 \"{tournament.Name}\" схвалено!",
                    NotificationType.Tournament,
                    $"/tournaments/{eventId}"
                );
            }
        }

        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.BasketballEventParticipants
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.PlayerId == playerId, cancellationToken);

        var tournament = await _dbcontext.BasketballEvents.FindAsync(eventId);

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
                    $"Вашу заявку на участь у змаганні з баскетболу 3х3 \"{tournament?.Name}\" було відхилено.",
                    NotificationType.Tournament,
                    $"/tournaments/{eventId}"
                );
            }
        }
    }

    public async Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.BasketballEventParticipants
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.PlayerId == playerId, cancellationToken);

        if (participant != null)
        {
            _dbcontext.BasketballEventParticipants.Remove(participant);

            // Also remove from team if exists
            var teamWithPlayer = await _dbcontext.BasketballTeams
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
            $"UPDATE players SET basketball_team_id = NULL WHERE basketball_team_id IN (SELECT \"Id\" FROM basketball3x3_teams WHERE \"EventId\" = {id})", 
            cancellationToken);

        // 1. Fetch event
        var entity = await _dbcontext.BasketballEvents
            .Include(e => e.Matches)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity == null) throw new EntityNotFoundException(nameof(BasketballEvent), id);

        // 2. Clear winners
        entity.FirstPlaceTeamId = null;
        entity.SecondPlaceTeamId = null;
        entity.ThirdPlaceTeamId = null;
        await _dbcontext.SaveChangesAsync(cancellationToken);

        // 3. Delete matches
        if (entity.Matches != null && entity.Matches.Any())
        {
            _dbcontext.BasketballMatches.RemoveRange(entity.Matches);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }

        // 4. Delete event
        _dbcontext.BasketballEvents.Remove(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }
}
