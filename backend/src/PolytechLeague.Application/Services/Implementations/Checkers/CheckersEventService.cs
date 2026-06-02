using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Entities.Shared;
using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Application.Dto.CheckersDto;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Checkers;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Exceptions;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Utilities;
using PolytechLeague.Domain.Entities.Enums;

namespace PolytechLeague.Application.Services.Implementations.Checkers;

public class CheckersEventService :
    AbstractEntityService<CheckersEvent>,
    ICheckersEventService
{
    private readonly IExcelImportService _excelImportService;
 
     public CheckersEventService(
         ApplicationDbContext dbcontext,
         ILogger<AbstractEntityService<CheckersEvent>> logger,
         INotificationService notificationService,
         IExcelImportService excelImportService
     ) : base(dbcontext, logger, notificationService)
     {
         _excelImportService = excelImportService;
     }
    protected override void UpdateValues(CheckersEvent updatedEntity, CheckersEvent newEntity)
    {
        updatedEntity.Name = newEntity.Name;
        updatedEntity.Date = newEntity.Date;
        updatedEntity.Location = newEntity.Location;
        updatedEntity.Description = newEntity.Description;
        updatedEntity.RegistrationStart = newEntity.RegistrationStart;
        updatedEntity.RegistrationEnd = newEntity.RegistrationEnd;
        updatedEntity.EndDate = newEntity.EndDate;
        updatedEntity.Level = newEntity.Level;
        updatedEntity.TimeControl = newEntity.TimeControl;
        updatedEntity.System = newEntity.System;
        updatedEntity.Variant = newEntity.Variant;
        updatedEntity.Rules = newEntity.Rules;
        updatedEntity.Prizes = newEntity.Prizes;
        updatedEntity.HeadRefereeName = newEntity.HeadRefereeName;
        updatedEntity.Referees = newEntity.Referees;
        updatedEntity.Schedule = newEntity.Schedule;
        updatedEntity.RefereeIds = newEntity.RefereeIds;
        updatedEntity.FirstPlaceId = newEntity.FirstPlaceId;
        updatedEntity.SecondPlaceId = newEntity.SecondPlaceId;
        updatedEntity.ThirdPlaceId = newEntity.ThirdPlaceId;
        updatedEntity.FirstPlaceTeamId = newEntity.FirstPlaceTeamId;
        updatedEntity.SecondPlaceTeamId = newEntity.SecondPlaceTeamId;
        updatedEntity.ThirdPlaceTeamId = newEntity.ThirdPlaceTeamId;
        updatedEntity.IsTeam = newEntity.IsTeam;
        updatedEntity.IsModerated = newEntity.IsModerated;
        updatedEntity.IsPublic = newEntity.IsPublic;
        updatedEntity.SeasonId = newEntity.SeasonId;
        updatedEntity.MvpId = newEntity.MvpId;
        updatedEntity.Category = newEntity.Category;

    }

    public override async Task<CheckersEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbcontext.Set<CheckersEvent>()
            .Include(e => e.Players!)
                .ThenInclude(p => p.Player)
            .Include(e => e.Games!)
                .ThenInclude(g => g.WhitePlayer!)
                    .ThenInclude(p => p.Player)
            .Include(e => e.Games!)
                .ThenInclude(g => g.BlackPlayer!)
                    .ThenInclude(p => p.Player)
            .Include(e => e.Games!)
                .ThenInclude(g => g.WhiteTeam)
            .Include(e => e.Games!)
                .ThenInclude(g => g.BlackTeam)
            .Include(e => e.Photos)
            .Include(e => e.Teams!)
                .ThenInclude(t => t.Players)
            .Include(e => e.FirstPlace)
            .Include(e => e.SecondPlace)
            .Include(e => e.ThirdPlace)
            .Include(e => e.FirstPlaceTeam)
            .Include(e => e.SecondPlaceTeam)
            .Include(e => e.ThirdPlaceTeam)
            .Include(e => e.Mvp)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)

            ?? throw new EntityNotFoundException(nameof(CheckersEvent), id);
    }

    public async Task AddParticipantAsync(long id, CheckersEventParticipantDto request, CancellationToken cancellationToken = default)
    {
        var tournament = await _dbcontext.Set<CheckersEvent>().FindAsync(new object[] { id }, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(CheckersEvent), id);

        if (tournament.Level == CompetitionLevel.CollegeGroup && request.BoardRank.HasValue)
        {
            throw new InvalidOperationException("Individual board ranks are not allowed for college group tournaments.");
        }

        var exists = await _dbcontext.Set<CheckersEventParticipant>()
            .AnyAsync(p => p.TournamentId == id && p.PlayerId == request.PlayerId, cancellationToken);
        
        if (exists) return;
 
        var player = await _dbcontext.Set<Player>()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == request.PlayerId, cancellationToken);
            
        if (player?.User?.Role == UserRole.Admin)
        {
            throw new InvalidOperationException("Administrators cannot be participants in tournaments.");
        }

        var entity = request.FromDto(id);
        if (string.IsNullOrEmpty(entity.GroupName))
        {
            entity.GroupName = player?.Group;
        }
        
        entity.PlayerFirstName = request.FirstName ?? player?.FirstName;
        entity.PlayerLastName = request.LastName ?? player?.LastName;

        await _dbcontext.Set<CheckersEventParticipant>().AddAsync(entity, cancellationToken);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateParticipantAsync(long id, long playerId, CheckersEventParticipantDto request, CancellationToken cancellationToken = default)
    {
        var tournament = await _dbcontext.Set<CheckersEvent>().FindAsync(new object[] { id }, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(CheckersEvent), id);

        if (tournament.Level == CompetitionLevel.CollegeGroup && request.BoardRank.HasValue)
        {
            throw new InvalidOperationException("Individual board ranks are not allowed for college group tournaments.");
        }

        var participant = await _dbcontext.Set<CheckersEventParticipant>()
            .Include(p => p.Player)
            .FirstOrDefaultAsync(p => p.TournamentId == id && p.PlayerId == playerId, cancellationToken);
            
        if (participant == null) throw new EntityNotFoundException("Учасника не знайдено", playerId);
 
        var oldStatus = participant.Status;
        participant.Points = request.Points;
        participant.Rank = request.Rank;
        participant.BoardRank = request.BoardRank;
        participant.Status = request.Status;
        participant.GroupName = request.GroupName;

        if (participant.Player != null)
        {
            if (!string.IsNullOrEmpty(request.FirstName)) participant.PlayerFirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName)) participant.PlayerLastName = request.LastName;
            participant.GroupName = request.GroupName;
        }

        _dbcontext.Set<CheckersEventParticipant>().Update(participant);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        if (oldStatus != request.Status && (request.Status == RegistrationStatus.Approved || request.Status == RegistrationStatus.Rejected))
        {
            await SendParticipantStatusNotificationAsync(id, playerId, request.Status, cancellationToken);
        }
    }

    public async Task RemoveParticipantAsync(long id, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<CheckersEventParticipant>()
            .FirstOrDefaultAsync(p => p.TournamentId == id && p.PlayerId == playerId, cancellationToken);
        if (participant != null)
        {
            _dbcontext.Set<CheckersEventParticipant>().Remove(participant);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task AddPhotoAsync(long eventId, string url, CancellationToken cancellationToken = default)
    {
        var photo = new PolytechLeague.Domain.Entities.EventPhotos
        {
            EventId = eventId,
            Url = url
        };
        _dbcontext.Set<PolytechLeague.Domain.Entities.EventPhotos>().Add(photo);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemovePhotoAsync(long eventId, long photoId, CancellationToken cancellationToken = default)
    {
        var photo = await _dbcontext.Set<PolytechLeague.Domain.Entities.EventPhotos>()
            .FirstOrDefaultAsync(p => p.Id == photoId && p.EventId == eventId, cancellationToken);
        if (photo != null)
        {
            _dbcontext.Set<PolytechLeague.Domain.Entities.EventPhotos>().Remove(photo);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }
    }

    protected override async Task OnCreatedAsync(CheckersEvent entity, CancellationToken cancellationToken)
    {
        await _notificationService.SendNotificationToAllAsync(
            "Нове змагання!",
            $"Створено нове змагання з шашок: {entity.Name}",
            NotificationType.Tournament,
            $"/tournaments/{entity.Id}",
            true
        );
    }

    protected override async Task OnUpdatedAsync(CheckersEvent originalEntity, CheckersEvent updatedEntity, CancellationToken cancellationToken)
    {
        bool individualWinnersSet = updatedEntity.FirstPlaceId != null && updatedEntity.SecondPlaceId != null && updatedEntity.ThirdPlaceId != null && (originalEntity.FirstPlaceId == null || originalEntity.SecondPlaceId == null || originalEntity.ThirdPlaceId == null);
        bool teamWinnersSet = updatedEntity.FirstPlaceTeamId != null && updatedEntity.SecondPlaceTeamId != null && updatedEntity.ThirdPlaceTeamId != null && (originalEntity.FirstPlaceTeamId == null || originalEntity.SecondPlaceTeamId == null || originalEntity.ThirdPlaceTeamId == null);
        
        if (individualWinnersSet || teamWinnersSet)
        {
            await _notificationService.SendNotificationToAllAsync(
                "Змагання завершено!",
                $"Визначено переможців у змаганні з шашок: {updatedEntity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{updatedEntity.Id}",
                true
            );
        }
    }
    public async Task ImportDataFromUrlAsync(long eventId, string url, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Excel data import for checkers event {EventId} from URL: {Url}", eventId, url);
        var data = await _excelImportService.ImportFromChessResultsUrlAsync(url, cancellationToken);
        await ProcessImportData(eventId, data, cancellationToken);
    }

    public async Task ImportDataFromFileAsync(long eventId, Stream stream, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Excel data import for checkers event {EventId} from uploaded file", eventId);
        var data = await _excelImportService.ImportFromStreamAsync(stream, cancellationToken);
        await ProcessImportData(eventId, data, cancellationToken);
    }

    private async Task ProcessImportData(long eventId, ScrapedTournamentExcelData data, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing {PlayerCount} players for checkers event {EventId}", 
            data.Players.Count, eventId);

        if (data.Players.Count == 0)
        {
            throw new Exception("Не вдалося знайти список учасників або дані в Excel. Перевірте посилання.");
        }

        foreach (var sp in data.Players)
        {
            Player? player = null;

            // 1. Try match by Name and Specialty
            var names = sp.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (names.Length >= 2)
            {
                var lastName = names[0];
                var firstName = names[1];
                
                var query = _dbcontext.Set<Player>()
                    .Where(p => p.LastName.ToLower() == lastName.ToLower() && p.FirstName.ToLower() == firstName.ToLower());

                if (!string.IsNullOrEmpty(sp.Group))
                {
                    var parsed = GroupUtility.ParseGroup(sp.Group, DateTime.Now);
                    if (parsed.specialty > 0)
                    {
                        player = await query.FirstOrDefaultAsync(p => p.SpecialtyCode == parsed.specialty && p.EnrollmentYear == parsed.enrollmentYear, cancellationToken);
                    }
                }

                if (player == null)
                {
                    player = await query.FirstOrDefaultAsync(cancellationToken);
                }
            }

            // 2. Create if not found
            if (player == null)
            {
                names = sp.FullName.Replace(",", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var lastName = names.Length > 0 ? names[0] : "Невідомий";
                var firstName = names.Length > 1 ? names[1] : "Гравець";

                var parsed = !string.IsNullOrEmpty(sp.Group) ? GroupUtility.ParseGroup(sp.Group, DateTime.Now) : (specialty: 0, enrollmentYear: 0, groupNumber: 0);
                
                player = new Player
                {
                    LastName = lastName,
                    FirstName = firstName,
                    City = "Чернівці",
                    Group = sp.Group ?? "Unknown",
                    EnrollmentYear = parsed.enrollmentYear,
                    SpecialtyCode = parsed.specialty,
                    GroupNumber = parsed.groupNumber,
                    PhotoUrl = null
                };
                _dbcontext.Set<Player>().Add(player);
                await _dbcontext.SaveChangesAsync(cancellationToken);
            }

            // Add as participant if not already
            await AddParticipantAsync(eventId, new CheckersEventParticipantDto
            {
                PlayerId = player.Id,
                Points = sp.Points,
                Rank = sp.Rank ?? 0,
                GroupName = sp.Group ?? player.Group,
                Status = RegistrationStatus.Approved
            }, cancellationToken);
        }

        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task RegisterPlayerAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var tournament = await _dbcontext.Set<CheckersEvent>().FindAsync([eventId], cancellationToken)
            ?? throw new EntityNotFoundException(nameof(CheckersEvent), eventId);

        if (tournament.Level != CompetitionLevel.CollegeGroup)
        {
            throw new InvalidOperationException("Self-registration is only allowed for college group tournaments.");
        }

        var player = await _dbcontext.Set<Player>()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == playerId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Player), playerId);

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

        var exists = await _dbcontext.Set<CheckersEventParticipant>()
            .AnyAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);

        if (exists) return;

        var status = (tournament.IsModerated && !tournament.IsPublic) ? RegistrationStatus.Pending : RegistrationStatus.Approved;

        var participant = new CheckersEventParticipant
        {
            TournamentId = eventId,
            PlayerId = playerId,
            Status = status,
            GroupName = player.Group,
            PlayerFirstName = player.FirstName,
            PlayerLastName = player.LastName
        };

        _dbcontext.Set<CheckersEventParticipant>().Add(participant);
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
        var participant = await _dbcontext.Set<CheckersEventParticipant>()
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
                    $"Вашу заявку на участь у змаганні з шашок схвалено!",
                    NotificationType.Tournament,
                    $"/tournaments/{eventId}"
                );
            }
        }
    }

    public async Task RejectParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<CheckersEventParticipant>()
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
                    $"Вашу заявку на участь у змаганні з шашок було відхилено.",
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
            ? "Вашу заявку на участь у змаганні з шашок схвалено!"
            : "Вашу заявку на участь у змаганні з шашок було відхилено.";

        await _notificationService.SendNotificationToUserAsync(
            player.UserId.Value,
            title,
            message,
            NotificationType.Tournament,
            $"/tournaments/{eventId}"
        );
    }

    public override async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        // 0. Clear player references to avoid FK constraint violation
        await _dbcontext.Database.ExecuteSqlRawAsync(
            $"UPDATE players SET \"CheckersTeamId\" = NULL WHERE \"CheckersTeamId\" IN (SELECT \"Id\" FROM checkers_teams WHERE \"EventId\" = {id})", 
            cancellationToken);

        // 1. Fetch event
        var entity = await _dbcontext.Set<CheckersEvent>()
            .Include(e => e.Games)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity == null) throw new EntityNotFoundException(nameof(CheckersEvent), id);

        // 2. Clear winners to break circular references
        entity.FirstPlaceId = null;
        entity.SecondPlaceId = null;
        entity.ThirdPlaceId = null;
        entity.FirstPlaceTeamId = null;
        entity.SecondPlaceTeamId = null;
        entity.ThirdPlaceTeamId = null;
        await _dbcontext.SaveChangesAsync(cancellationToken);

        // 3. Delete games
        if (entity.Games != null && entity.Games.Any())
        {
            _dbcontext.Set<CheckersGame>().RemoveRange(entity.Games);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }

        // 4. Delete event
        _dbcontext.Set<CheckersEvent>().Remove(entity);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }
}

