using PolytechLeague.Domain.Entities.TableTennis;
using PolytechLeague.Domain.Entities.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Persistence.Database;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Application.Dto.TableTennisDto;
using PolytechLeague.Domain.Entities.Enums;

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
        oldEntity.RegistrationStart = newEntity.RegistrationStart;
        oldEntity.RegistrationEnd = newEntity.RegistrationEnd;
        oldEntity.EndDate = newEntity.EndDate;
        oldEntity.Level = newEntity.Level;
        oldEntity.TournamentType = newEntity.TournamentType;
        oldEntity.Category = newEntity.Category;
        oldEntity.IsModerated = newEntity.IsModerated;
        oldEntity.IsPublic = newEntity.IsPublic;
        oldEntity.IsTeam = newEntity.IsTeam;
        oldEntity.FirstPlaceId = newEntity.FirstPlaceId;
        oldEntity.SecondPlaceId = newEntity.SecondPlaceId;
        oldEntity.ThirdPlaceId = newEntity.ThirdPlaceId;
        oldEntity.FirstPlaceTeamId = newEntity.FirstPlaceTeamId;
        oldEntity.SecondPlaceTeamId = newEntity.SecondPlaceTeamId;
        oldEntity.ThirdPlaceTeamId = newEntity.ThirdPlaceTeamId;
        oldEntity.SeasonId = newEntity.SeasonId;

        oldEntity.UpdatedAt = DateTime.UtcNow;
    }

    public override async Task<TableTennisEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbcontext.TableTennisEvents
            .Include(e => e.Photos)
            .Include(e => e.FirstPlace)
            .Include(e => e.SecondPlace)
            .Include(e => e.ThirdPlace)
            .Include(e => e.FirstPlaceTeam)
            .Include(e => e.SecondPlaceTeam)
            .Include(e => e.ThirdPlaceTeam)
            .Include(e => e.Teams!)
                .ThenInclude(t => t.Players)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.Player1)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.Player2)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.HomeTeam)
            .Include(e => e.Matches!)
                .ThenInclude(m => m.AwayTeam)
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
            $"/tournaments/{entity.Id}",
            true
        );
    }

    protected override async Task OnUpdatedAsync(TableTennisEvent oldEntity, TableTennisEvent newEntity, CancellationToken cancellationToken)
    {
        // Update ranks for teams based on winners
        var eventWithTeams = await _dbcontext.TableTennisEvents
            .Include(e => e.Teams)
            .FirstOrDefaultAsync(e => e.Id == newEntity.Id, cancellationToken);

        if (eventWithTeams != null && eventWithTeams.Teams != null)
        {
            foreach (var team in eventWithTeams.Teams)
            {
                if (team.Id == newEntity.FirstPlaceTeamId) team.Rank = 1;
                else if (team.Id == newEntity.SecondPlaceTeamId) team.Rank = 2;
                else if (team.Id == newEntity.ThirdPlaceTeamId) team.Rank = 3;
                else if (newEntity.FirstPlaceTeamId != null) team.Rank = 4;
                else team.Rank = 0;
            }
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }

        bool individualWinnersSet = newEntity.FirstPlaceId != null && newEntity.SecondPlaceId != null && newEntity.ThirdPlaceId != null;
        bool teamWinnersSet = newEntity.FirstPlaceTeamId != null && newEntity.SecondPlaceTeamId != null && newEntity.ThirdPlaceTeamId != null;
        
        if (individualWinnersSet || teamWinnersSet)
        {
            await _notificationService.SendNotificationToAllAsync(
                "Змагання завершено!",
                $"Визначено переможців у змаганні з настільного тенісу: {newEntity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{newEntity.Id}",
                true
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
            Rank = dto.Rank,
            GroupName = dto.GroupName,
            Status = dto.Status
        };

        var player = await _dbcontext.Set<Player>().FindAsync(dto.PlayerId);
        if (string.IsNullOrEmpty(participant.GroupName))
        {
            participant.GroupName = player?.Group;
        }
        
        participant.PlayerFirstName = dto.FirstName ?? player?.FirstName;
        participant.PlayerLastName = dto.LastName ?? player?.LastName;

        _dbcontext.Set<TableTennisEventParticipant>().Add(participant);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        if (dto.Status == RegistrationStatus.Approved || dto.Status == RegistrationStatus.Rejected)
        {
            await SendParticipantStatusNotificationAsync(eventId, dto.PlayerId, dto.Status, cancellationToken);
        }
    }

    public async Task UpdateParticipantAsync(long eventId, long playerId, TableTennisEventParticipantDto dto, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<TableTennisEventParticipant>()
            .FirstOrDefaultAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);
        if (participant != null)
        {
            var oldStatus = participant.Status;
            participant.Points = dto.Points;
            participant.Rank = dto.Rank;
            participant.Status = dto.Status;
            participant.GroupName = dto.GroupName;
            
            if (!string.IsNullOrEmpty(dto.FirstName)) participant.PlayerFirstName = dto.FirstName;
            if (!string.IsNullOrEmpty(dto.LastName)) participant.PlayerLastName = dto.LastName;

            _dbcontext.Set<TableTennisEventParticipant>().Update(participant);
            await _dbcontext.SaveChangesAsync(cancellationToken);

            if (oldStatus != dto.Status && (dto.Status == RegistrationStatus.Approved || dto.Status == RegistrationStatus.Rejected))
            {
                await SendParticipantStatusNotificationAsync(eventId, playerId, dto.Status, cancellationToken);
            }
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

    public async Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var tournament = await _dbcontext.TableTennisEvents.FindAsync([eventId], cancellationToken)
            ?? throw new PolytechLeague.Domain.Exceptions.EntityNotFoundException(nameof(TableTennisEvent), eventId);

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
            throw new InvalidOperationException("Для реєстрації на змагання необхідно вказати серію та номер студентського квитка в профілі (наприклад: PH13856483).");
        }

        if (player.Gender == Gender.None)
        {
            throw new InvalidOperationException("Для реєстрації на змагання необхідно вказати стать у профілі.");
        }

        var exists = await _dbcontext.Set<TableTennisEventParticipant>()
            .AnyAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);

        if (exists) return;

        var status = (tournament.IsModerated && !tournament.IsPublic) ? RegistrationStatus.Pending : RegistrationStatus.Approved;

        var participant = new TableTennisEventParticipant
        {
            TournamentId = eventId,
            PlayerId = playerId,
            Status = status,
            GroupName = player.Group,
            PlayerFirstName = player.FirstName,
            PlayerLastName = player.LastName
        };

        _dbcontext.Set<TableTennisEventParticipant>().Add(participant);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        if (tournament.IsModerated)
        {
            await _notificationService.SendNotificationToAdminsAsync(
                "Новий запит на реєстрацію",
                $"Гравець {player?.FirstName} {player?.LastName} хоче зареєструватися на {tournament.Name}",
                NotificationType.Tournament,
                $"/tournaments/{eventId}"
            );
        }
    }

    public async Task ApproveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<TableTennisEventParticipant>()
            .FirstOrDefaultAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);

        if (participant != null)
        {
            participant.Status = RegistrationStatus.Approved;
            await _dbcontext.SaveChangesAsync(cancellationToken);

            var player = await _dbcontext.Set<Player>().FindAsync([playerId], cancellationToken);
            if (player?.UserId != null)
            {
                await _notificationService.SendNotificationToUserAsync(
                    player.UserId.Value,
                    "Реєстрація схвалена",
                    $"Вашу заявку на участь у змаганні з настільного тенісу схвалено!",
                    NotificationType.Tournament,
                    $"/tournaments/{eventId}"
                );
            }
        }
    }

    public async Task RejectParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<TableTennisEventParticipant>()
            .FirstOrDefaultAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);

        if (participant != null)
        {
            participant.Status = RegistrationStatus.Rejected;
            await _dbcontext.SaveChangesAsync(cancellationToken);

            var player = await _dbcontext.Set<Player>().FindAsync([playerId], cancellationToken);
            if (player?.UserId != null)
            {
                await _notificationService.SendNotificationToUserAsync(
                    player.UserId.Value,
                    "Реєстрація відхилена",
                    $"Вашу заявку на участь у змаганні з настільного тенісу було відхилено.",
                    NotificationType.Tournament,
                    $"/tournaments/{eventId}"
                );
            }
        }
    }

    private async Task SendParticipantStatusNotificationAsync(long eventId, long playerId, RegistrationStatus status, CancellationToken cancellationToken)
    {
        var player = await _dbcontext.Set<Player>().FindAsync([playerId], cancellationToken);
        if (player?.UserId == null) return;

        string title = status == RegistrationStatus.Approved ? "Реєстрація схвалена" : "Реєстрація відхилена";
        string message = status == RegistrationStatus.Approved
            ? "Вашу заявку на участь у змаганні з настільного тенісу схвалено!"
            : "Вашу заявку на участь у змаганні з настільного тенісу було відхилено.";

        await _notificationService.SendNotificationToUserAsync(
            player.UserId.Value,
            title,
            message,
            NotificationType.Tournament,
            $"/tournaments/{eventId}"
        );
    }
}
