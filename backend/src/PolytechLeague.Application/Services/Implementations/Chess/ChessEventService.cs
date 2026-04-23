using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Entities.Chess;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Services.Interfaces.Chess;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Exceptions;
using PolytechLeague.Application.Dto;

namespace PolytechLeague.Application.Services.Implementations.Chess;

public class ChessEventService(
    ApplicationDbContext _dbcontext,
    ILogger<AbstractEntityService<ChessEvent>> _logger,
    INotificationService _notificationService,
    IExcelImportService _excelImportService
) :
    AbstractEntityService<ChessEvent>(_dbcontext, _logger, _notificationService),
    IChessEventService
{
    protected override void UpdateValues(ChessEvent oldEntity, ChessEvent newEntity)
    {
        oldEntity.Name = newEntity.Name;
        oldEntity.Date = newEntity.Date;
        oldEntity.Location = newEntity.Location;
        oldEntity.Description = newEntity.Description;
        oldEntity.RegistrationStart = newEntity.RegistrationStart;
        oldEntity.RegistrationEnd = newEntity.RegistrationEnd;
        oldEntity.EndDate = newEntity.EndDate;
        oldEntity.Level = newEntity.Level;
        oldEntity.TimeControl = newEntity.TimeControl;
        oldEntity.System = newEntity.System;
        oldEntity.Rules = newEntity.Rules;
        oldEntity.Prizes = newEntity.Prizes;
        oldEntity.HeadRefereeName = newEntity.HeadRefereeName;
        oldEntity.Referees = newEntity.Referees;
        oldEntity.RefereeIds = newEntity.RefereeIds;
        oldEntity.Schedule = newEntity.Schedule;
        oldEntity.FirstPlaceId = newEntity.FirstPlaceId;
        oldEntity.SecondPlaceId = newEntity.SecondPlaceId;
        oldEntity.ThirdPlaceId = newEntity.ThirdPlaceId;
        oldEntity.FirstPlaceTeamId = newEntity.FirstPlaceTeamId;
        oldEntity.SecondPlaceTeamId = newEntity.SecondPlaceTeamId;
        oldEntity.ThirdPlaceTeamId = newEntity.ThirdPlaceTeamId;
        oldEntity.IsTeam = newEntity.IsTeam;
    }

    public override async Task<ChessEvent> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbcontext.Set<ChessEvent>()
            .Include(e => e.Photos)
            .Include(e => e.Players)
                .ThenInclude(p => p.Player)
            .Include(e => e.Teams)
                .ThenInclude(t => t.Players)
            .Include(e => e.Games)
                .ThenInclude(g => g.WhitePlayer)
            .Include(e => e.Games)
                .ThenInclude(g => g.BlackPlayer)
            .Include(e => e.Games)
                .ThenInclude(g => g.WhiteTeam)
            .Include(e => e.Games)
                .ThenInclude(g => g.BlackTeam)
            .Include(e => e.FirstPlace)
            .Include(e => e.SecondPlace)
            .Include(e => e.ThirdPlace)
            .Include(e => e.FirstPlaceTeam)
            .Include(e => e.SecondPlaceTeam)
            .Include(e => e.ThirdPlaceTeam)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(ChessEvent), id);
    }

    public async Task AddParticipantAsync(long eventId, PolytechLeague.Application.Dto.ChessDto.ChessEventParticipantDto dto, CancellationToken cancellationToken = default)
    {
        var exists = await _dbcontext.Set<ChessEventParticipant>()
            .AnyAsync(p => p.TournamentId == eventId && p.PlayerId == dto.PlayerId, cancellationToken);

        if (exists) return;

        var participant = new ChessEventParticipant
        {
            TournamentId = eventId,
            PlayerId = dto.PlayerId,
            Points = dto.Points,
            Rank = dto.Rank
        };
        _dbcontext.Set<ChessEventParticipant>().Add(participant);
        await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateParticipantAsync(long eventId, long playerId, PolytechLeague.Application.Dto.ChessDto.ChessEventParticipantDto dto, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<ChessEventParticipant>()
            .FirstOrDefaultAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);
        if (participant != null)
        {
            participant.Points = dto.Points;
            participant.Rank = dto.Rank;
            // Removed Buchholz and SonnebornBerger from Entity since they are not mapped
            _dbcontext.Set<ChessEventParticipant>().Update(participant);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveParticipantAsync(long eventId, long playerId, CancellationToken cancellationToken = default)
    {
        var participant = await _dbcontext.Set<ChessEventParticipant>()
            .FirstOrDefaultAsync(p => p.TournamentId == eventId && p.PlayerId == playerId, cancellationToken);
        if (participant != null)
        {
            _dbcontext.Set<ChessEventParticipant>().Remove(participant);
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

    protected override async Task OnCreatedAsync(ChessEvent entity, CancellationToken cancellationToken)
    {
        await _notificationService.SendNotificationToAllAsync(
            "Нове змагання!",
            $"Створено нове змагання з шахів: {entity.Name}",
            NotificationType.Tournament,
            $"/tournaments/{entity.Id}"
        );
    }

    protected override async Task OnUpdatedAsync(ChessEvent oldEntity, ChessEvent newEntity, CancellationToken cancellationToken)
    {
        bool individualWinnersSet = newEntity.FirstPlaceId != null && newEntity.SecondPlaceId != null && newEntity.ThirdPlaceId != null;
        bool teamWinnersSet = newEntity.FirstPlaceTeamId != null && newEntity.SecondPlaceTeamId != null && newEntity.ThirdPlaceTeamId != null;
        
        if (individualWinnersSet || teamWinnersSet)
        {
            await _notificationService.SendNotificationToAllAsync(
                "Змагання завершено!",
                $"Визначено переможців у змаганні з шахів: {newEntity.Name}",
                NotificationType.Tournament,
                $"/tournaments/{newEntity.Id}"
            );
        }
    }

    public async Task ImportDataFromUrlAsync(long eventId, string url, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Excel data import for event {EventId} from URL: {Url}", eventId, url);
        var data = await _excelImportService.ImportFromChessResultsUrlAsync(url, cancellationToken);
        await ProcessImportData(eventId, data, cancellationToken);
    }

    public async Task ImportDataFromFileAsync(long eventId, Stream stream, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Excel data import for event {EventId} from uploaded file", eventId);
        var data = await _excelImportService.ImportFromStreamAsync(stream, cancellationToken);
        await ProcessImportData(eventId, data, cancellationToken);
    }

    private async Task ProcessImportData(long eventId, ScrapedTournamentExcelData data, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing {PlayerCount} players for event {EventId}", 
            data.Players.Count, eventId);

        if (data.Players.Count == 0)
        {
            throw new Exception("Не вдалося знайти список учасників або дані в Excel. Перевірте посилання.");
        }

        foreach (var sp in data.Players)
        {
            Player? player = null;
            string[] names;

            // 1. Try match by FIDE ID
            if (!string.IsNullOrEmpty(sp.FideId))
            {
                player = await _dbcontext.Set<Player>()
                    .FirstOrDefaultAsync(p => p.ChessFideId == sp.FideId, cancellationToken);
            }

            // 2. Try match by Name
            if (player == null)
            {
                names = sp.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (names.Length >= 2)
                {
                    var lastName = names[0];
                    var firstName = names[1];
                    player = await _dbcontext.Set<Player>()
                        .FirstOrDefaultAsync(p => p.LastName.ToLower() == lastName.ToLower() && p.FirstName.ToLower() == firstName.ToLower(), cancellationToken);
                }
            }

            // 3. Create if not found
            if (player == null)
            {
                names = sp.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var lastName = names.Length > 0 ? names[0] : "Невідомий";
                var firstName = names.Length > 1 ? names[1] : "Гравець";

                player = new Player
                {
                    LastName = lastName,
                    FirstName = firstName,
                    City = "Чернівці",
                    Group = "Unknown",
                    PhotoUrl = "/images/placeholders/user.png",
                    ChessFideId = sp.FideId,
                    ChessRating = sp.Rating,
                    ChessTitle = sp.Title
                };
                _dbcontext.Set<Player>().Add(player);
                await _dbcontext.SaveChangesAsync(cancellationToken);
            }

            // Add as participant if not already
            await AddParticipantAsync(eventId, new PolytechLeague.Application.Dto.ChessDto.ChessEventParticipantDto
            {
                PlayerId = player.Id,
                Points = sp.Points,
                Rank = sp.Rank ?? 0
            }, cancellationToken);
        }

        await _dbcontext.SaveChangesAsync(cancellationToken);
    }
}
