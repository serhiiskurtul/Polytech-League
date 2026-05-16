using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Entities.Chess;
using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Domain.Entities.TableTennis;
using PolytechLeague.Domain.Entities.Badminton;
using PolytechLeague.Domain.Entities.Armwrestling;
using PolytechLeague.Domain.Entities.Armwrestling.Enums;
using PolytechLeague.Domain.Entities.Football;
using PolytechLeague.Domain.Entities.Volleyball;
using PolytechLeague.Domain.Entities.Basketball3x3;
using PolytechLeague.Domain.Entities.BeachVolleyball;
using PolytechLeague.Domain.Entities.Athletics;
using PolytechLeague.Domain.Entities.Shared;
using PolytechLeague.Domain.Entities.Enums;
using PolytechLeague.Application.Utilities;
namespace PolytechLeague.Application.Services.Implementations.General;

public class PlayerService : IPlayerService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PlayerService> _logger;
    private readonly INotificationService _notificationService;

    private static readonly Dictionary<long, (DateTime ExpiredAt, IEnumerable<PlayerDto> Data)> _rankingsCache = new();
    private static readonly object _cacheLock = new object();
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);


    public PlayerService(ApplicationDbContext dbContext, ILogger<PlayerService> logger, INotificationService notificationService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<PlayerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var players = await _dbContext.Players
            .AsNoTracking()
            .Include(p => p.User)
            .Where(p => p.User == null || p.User.Role != UserRole.Admin)
            .ToListAsync(cancellationToken);
        return players.Select(p => p.ToDto());
    }

    public async Task<PlayerDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var allWithRankings = await GetAllWithRankingsAsync(null, cancellationToken);
        var ranked = allWithRankings.FirstOrDefault(p => p.Id == id);
        if (ranked != null) { 
            await PopulateAchievementsAsync(ranked, cancellationToken);
            await PopulateAutomaticBadgesAsync(ranked, cancellationToken); 
            await PopulateChessSpecialStatsAsync(ranked, cancellationToken); 
            await PopulateFootballStatsAsync(ranked, cancellationToken);
            return ranked; 
        }
        // Fallback for players not in rankings (Admins, External)
        var player = await _dbContext.Players
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (player == null) return null;
        var dto = player.ToDto();
        dto.HasPendingMergeRequest = await _dbContext.ProfileMergeRequests
            .AnyAsync(r => r.UserId == (player.UserId ?? 0) && r.Status == RegistrationStatus.Pending, cancellationToken);
        await PopulateAchievementsAsync(dto, cancellationToken); await PopulateAutomaticBadgesAsync(dto, cancellationToken); await PopulateChessSpecialStatsAsync(dto, cancellationToken);
        return dto;
    }

    public async Task<string?> GetPhotoUrlByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        var player = await _dbContext.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        return player?.PhotoUrl;
    }

    public async Task<PlayerDto?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        var allWithRankings = await GetAllWithRankingsAsync(null, cancellationToken);
        var ranked = allWithRankings.FirstOrDefault(p => p.UserId == userId);
        if (ranked != null) { 
            await PopulateAchievementsAsync(ranked, cancellationToken);
            await PopulateAutomaticBadgesAsync(ranked, cancellationToken); 
            await PopulateChessSpecialStatsAsync(ranked, cancellationToken); 
            await PopulateFootballStatsAsync(ranked, cancellationToken);
            return ranked; 
        }
        // Fallback for players not in rankings (Admins, External)
        var player = await _dbContext.Players
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        if (player == null) return null;
        var dto = player.ToDto();
        dto.HasPendingMergeRequest = await _dbContext.ProfileMergeRequests
            .AnyAsync(r => r.UserId == userId && r.Status == RegistrationStatus.Pending, cancellationToken);
        await PopulateAchievementsAsync(dto, cancellationToken); 
        await PopulateAutomaticBadgesAsync(dto, cancellationToken); 
        await PopulateChessSpecialStatsAsync(dto, cancellationToken);
        await PopulateFootballStatsAsync(dto, cancellationToken);
        return dto;
    }

    public async Task<PlayerDto> CreateAsync(PlayerDto dto, CancellationToken cancellationToken = default)
    {
        // Cleanup names
        dto.FirstName = dto.FirstName?.Replace(",", "").Trim();
        dto.LastName = dto.LastName?.Replace(",", "").Trim();
        // Try to prevent duplicates if specialty and enrollment year are provided or can be parsed
        if (dto.EnrollmentYear == 0 && !string.IsNullOrEmpty(dto.Group))
        {
            var parsed = GroupUtility.ParseGroup(dto.Group, DateTime.Now);
            dto.SpecialtyCode = parsed.specialty;
            dto.EnrollmentYear = parsed.enrollmentYear;
            dto.GroupNumber = parsed.groupNumber;
        }
        if (dto.EnrollmentYear > 0)
        {
            var existing = await _dbContext.Players
                .FirstOrDefaultAsync(p => p.FirstName.ToLower() == dto.FirstName!.ToLower() &&
                                          p.LastName.ToLower() == dto.LastName!.ToLower() &&
                                          p.SpecialtyCode == dto.SpecialtyCode &&
                                          p.EnrollmentYear == dto.EnrollmentYear, cancellationToken);
            if (existing != null)
            {
                _logger.LogInformation("Found existing player {PlayerId} for {Name} in group {Group}", existing.Id, dto.LastName, dto.Group);
                return existing.ToDto();
            }
        }
        var player = dto.FromDto();
        // Auto-populate group fields if not set but Group string is present
        if (player.EnrollmentYear == 0 && !string.IsNullOrEmpty(player.Group))
        {
            var parsed = GroupUtility.ParseGroup(player.Group, DateTime.Now);
            player.SpecialtyCode = parsed.specialty;
            player.EnrollmentYear = parsed.enrollmentYear;
            player.GroupNumber = parsed.groupNumber;
        }
        await _dbContext.Players.AddAsync(player, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return player.ToDto();
    }

    public async Task UpdateAsync(long id, PlayerDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating player {Id}. DTO Bio: {Bio}, Instagram: {Instagram}", id, dto.Bio, dto.InstagramUrl);
        var player = await _dbContext.Players.FindAsync(new object[] { id }, cancellationToken);
        if (player == null)
        {
            _logger.LogWarning("Player {Id} not found for update", id);
            throw new KeyNotFoundException($"Player with ID {id} not found");
        }
        // Cleanup names
        dto.FirstName = dto.FirstName?.Replace(",", "").Trim();
        dto.LastName = dto.LastName?.Replace(",", "").Trim();
        // Update basic info - only if provided (not null)
        if (dto.FirstName != null && !string.IsNullOrWhiteSpace(dto.FirstName)) player.FirstName = dto.FirstName;
        if (dto.Patronymic != null) player.Patronymic = dto.Patronymic;
        if (dto.LastName != null && !string.IsNullOrWhiteSpace(dto.LastName)) player.LastName = dto.LastName;
        if (dto.Group != null) player.Group = dto.Group;
        // Handle StudentId: treat empty/whitespace as null to avoid unique index issues with multiple empty strings
        if (dto.StudentId != null)
        {
            player.StudentId = string.IsNullOrWhiteSpace(dto.StudentId) ? null : dto.StudentId.Trim();
        }
        if (dto.City != null) player.City = dto.City;
        if (dto.UserId != null) player.UserId = dto.UserId;
        if (dto.StudyYears != null) player.StudyYears = dto.StudyYears;
        _logger.LogInformation("Updating Bio for player {Id}. Old: {Old}, New: {New}", id, player.Bio, dto.Bio);
        if (dto.Bio != null) player.Bio = dto.Bio;
        _logger.LogInformation("Updating InstagramUrl for player {Id}. Old: {Old}, New: {New}", id, player.InstagramUrl, dto.InstagramUrl);
        if (dto.InstagramUrl != null) player.InstagramUrl = dto.InstagramUrl;
        if (dto.PreviousNames != null) player.PreviousNames = dto.PreviousNames;
        if (dto.PhotoUrl != null) player.PhotoUrl = dto.PhotoUrl;
        if (dto.Gender != Gender.None) player.Gender = dto.Gender;
        // Sync name and photo to User entity if it exists
        if (player.UserId.HasValue)
        {
            var user = await _dbContext.Users.FindAsync(new object[] { player.UserId.Value }, cancellationToken);
            if (user != null)
            {
                user.FirstName = player.FirstName;
                user.LastName = player.LastName;
                user.PhotoUrl = player.PhotoUrl; // Sync photo URL
                _dbContext.Users.Update(user);
            }
        }
        // Explicitly mark player as updated to ensure TPT fields are saved
        _dbContext.Players.Update(player);
        // Update sport stats (only if provided in DTO)
        if (dto.ChessRating.HasValue) player.ChessRating = dto.ChessRating;
        if (dto.ChessFideId != null) player.ChessFideId = dto.ChessFideId;
        if (dto.ChessTitle != null) player.ChessTitle = dto.ChessTitle;
        if (dto.CheckersRating.HasValue) player.CheckersRating = dto.CheckersRating;
        if (dto.TableTennisRating.HasValue) player.TableTennisRating = dto.TableTennisRating;
        if (dto.TableTennisHand.HasValue) player.TableTennisHand = dto.TableTennisHand;
        if (dto.ArmwrestlingWeight.HasValue) player.ArmwrestlingWeight = dto.ArmwrestlingWeight;
        if (dto.ArmwrestlingPreferredWeightClass.HasValue) player.ArmwrestlingPreferredWeightClass = dto.ArmwrestlingPreferredWeightClass;
        // Update team stats
        if (dto.FootballPosition.HasValue) player.FootballPosition = dto.FootballPosition;
        if (dto.FootballShirtNumber.HasValue) player.FootballShirtNumber = dto.FootballShirtNumber;
        if (dto.FootballTeamId.HasValue) player.FootballTeamId = dto.FootballTeamId;
        if (dto.VolleyballPosition.HasValue) player.VolleyballPosition = dto.VolleyballPosition;
        if (dto.VolleyballShirtNumber.HasValue) player.VolleyballShirtNumber = dto.VolleyballShirtNumber;
        if (dto.VolleyballTeamId.HasValue) player.VolleyballTeamId = dto.VolleyballTeamId;
        if (dto.BasketballShirtNumber.HasValue) player.BasketballShirtNumber = dto.BasketballShirtNumber;
        if (dto.BasketballTeamId.HasValue) player.BasketballTeamId = dto.BasketballTeamId;
        if (dto.BeachVolleyballShirtNumber.HasValue) player.BeachVolleyballShirtNumber = dto.BeachVolleyballShirtNumber;
        if (dto.BeachVolleyballTeamId.HasValue) player.BeachVolleyballTeamId = dto.BeachVolleyballTeamId;
        player.IsExternal = dto.IsExternal;
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating player {PlayerId}: {Message}", id, ex.Message);
            throw; // Re-throw to let the controller handle it (it will return 500 but now we have logs)
        }
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var player = await _dbContext.Players.FindAsync(new object[] { id }, cancellationToken);
        if (player != null)
        {
            _dbContext.Players.Remove(player);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<PlayerDto>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var lowerQuery = query.ToLower();
        var players = await _dbContext.Players
            .AsNoTracking()
            .Include(p => p.User)
            .Where(p => p.User == null || p.User.Role != UserRole.Admin)
            .Where(p => p.FirstName.ToLower().Contains(lowerQuery) ||
                        (p.Patronymic != null && p.Patronymic.ToLower().Contains(lowerQuery)) ||
                        p.LastName.ToLower().Contains(lowerQuery) ||
                        (p.PreviousNames != null && p.PreviousNames.ToLower().Contains(lowerQuery)) ||
                        p.Group.ToLower().Contains(lowerQuery) ||
                        (p.StudentId != null && p.StudentId.ToLower().Contains(lowerQuery)))
            .ToListAsync(cancellationToken);
        // Also check if the query matches their calculated group name for other years
        var allPlayers = await _dbContext.Players
            .AsNoTracking()
            .Include(p => p.User)
            .Where(p => p.User == null || p.User.Role != UserRole.Admin)
            .ToListAsync(cancellationToken);
        var filtered = allPlayers.Where(p =>
            p.FirstName.ToLower().Contains(lowerQuery) ||
            (p.Patronymic != null && p.Patronymic.ToLower().Contains(lowerQuery)) ||
            p.LastName.ToLower().Contains(lowerQuery) ||
            (p.PreviousNames != null && p.PreviousNames.ToLower().Contains(lowerQuery)) ||
            p.Group.ToLower().Contains(lowerQuery) ||
            (p.EnrollmentYear > 0 && GroupUtility.GetCurrentGroupName(p.SpecialtyCode, p.EnrollmentYear, p.GroupNumber).ToLower().Contains(lowerQuery)) ||
            (p.StudentId != null && p.StudentId.ToLower().Contains(lowerQuery))
        );
        return filtered.Select(p => p.ToDto());
    }

    public async Task<IEnumerable<PlayerDto>> GetAllWithRankingsAsync(long? seasonId = null, CancellationToken cancellationToken = default)
    {
        long cacheKey = seasonId ?? 0;
        lock (_cacheLock)
        {
            if (_rankingsCache.TryGetValue(cacheKey, out var entry) && entry.ExpiredAt > DateTime.Now)
            {
                return entry.Data;
            }
        }

        var data = await GetAllWithRankingsInternalAsync(seasonId, cancellationToken);

        lock (_cacheLock)
        {
            _rankingsCache[cacheKey] = (DateTime.Now.Add(_cacheDuration), data);
        }

        return data;
    }

    private async Task<IEnumerable<PlayerDto>> GetAllWithRankingsInternalAsync(long? seasonId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var players = await _dbContext.Players
                .AsNoTracking()
                .Include(p => p.User)
                .Where(p => p.User == null || p.User.Role != UserRole.Admin)
                .Where(p => !p.IsExternal && !p.IsGuest)
                .ToListAsync(cancellationToken);
            if (!players.Any()) return Enumerable.Empty<PlayerDto>();
            var playerIds = players.Select(p => p.Id).ToList();
            // 1. Level multipliers
            var levelMultipliers = new Dictionary<CompetitionLevel, double> {
                { CompetitionLevel.CollegeGroup, 1.0 },
                { CompetitionLevel.Regional, 2.5 },
                { CompetitionLevel.National, 5.0 }
            };
            var allEventsQuery = _dbContext.Set<Event>().AsNoTracking();
            if (seasonId.HasValue)
            {
                var season = await _dbContext.Seasons.FindAsync(new object[] { seasonId.Value }, cancellationToken);
                if (season != null)
                {
                    allEventsQuery = allEventsQuery.Where(e => e.SeasonId == seasonId.Value || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate));
                }
                else
                {
                    allEventsQuery = allEventsQuery.Where(e => e.SeasonId == seasonId.Value);
                }
            }
            var allEventsData = await allEventsQuery
                .Select(e => new { e.Id, e.Level })
                .ToListAsync(cancellationToken);
            var allEvents = allEventsData.GroupBy(e => e.Id).ToDictionary(g => g.Key, g => g.First().Level);
            
            var eligibleEventIdsList = allEvents.Keys.ToList();
            var eligiblePlayerIdsList = players.Select(p => p.Id).Distinct().ToList();

            // 2. Achievements
            var chessAch = await _dbContext.ChessEventParticipants.AsNoTracking().Include(p => p.Tournament).Where(p => eligiblePlayerIdsList.Contains(p.PlayerId) && eligibleEventIdsList.Contains(p.TournamentId)).ToListAsync(cancellationToken);
            var checkersAch = await _dbContext.CheckersEventParticipants.AsNoTracking().Include(p => p.Event).Where(p => eligiblePlayerIdsList.Contains(p.PlayerId) && eligibleEventIdsList.Contains(p.TournamentId)).ToListAsync(cancellationToken);
            var ttAch = await _dbContext.TableTennisEventParticipants.AsNoTracking().Include(p => p.Tournament).Where(p => eligiblePlayerIdsList.Contains(p.PlayerId) && eligibleEventIdsList.Contains(p.TournamentId)).ToListAsync(cancellationToken);
            var badmintonAch = await _dbContext.BadmintonEventParticipants.AsNoTracking().Include(p => p.Event).Where(p => eligiblePlayerIdsList.Contains(p.PlayerId) && eligibleEventIdsList.Contains(p.EventId)).ToListAsync(cancellationToken);
            var armwAch = await _dbContext.ArmwrestlingEventParticipants.AsNoTracking().Include(p => p.Tournament).Where(p => eligiblePlayerIdsList.Contains(p.PlayerId) && eligibleEventIdsList.Contains(p.TournamentId)).ToListAsync(cancellationToken);
            var genericWinners = await _dbContext.Set<Event>().AsNoTracking()
                .Where(e => eligibleEventIdsList.Contains(e.Id) &&
                            ((e.FirstPlaceId.HasValue && eligiblePlayerIdsList.Contains(e.FirstPlaceId.Value)) ||
                             (e.SecondPlaceId.HasValue && eligiblePlayerIdsList.Contains(e.SecondPlaceId.Value)) ||
                             (e.ThirdPlaceId.HasValue && eligiblePlayerIdsList.Contains(e.ThirdPlaceId.Value)) ||
                             (e.MvpId.HasValue && eligiblePlayerIdsList.Contains(e.MvpId.Value))))
                .ToListAsync(cancellationToken);
            var FootballTeams = await _dbContext.FootballTeams.AsNoTracking().Include(t => t.Event).Include(t => t.Players).Where(t => eligibleEventIdsList.Contains(t.EventId) && t.Players!.Any(p => eligiblePlayerIdsList.Contains(p.Id))).ToListAsync(cancellationToken);
            var volleyballTeams = await _dbContext.VolleyballTeams.AsNoTracking().Include(t => t.Event).Include(t => t.Players).Where(t => eligibleEventIdsList.Contains(t.EventId) && t.Players!.Any(p => eligiblePlayerIdsList.Contains(p.Id))).ToListAsync(cancellationToken);
            var bballTeams = await _dbContext.Basketball3x3Teams.AsNoTracking().Include(t => t.Event).Include(t => t.Players).Where(t => eligibleEventIdsList.Contains(t.EventId) && t.Players!.Any(p => eligiblePlayerIdsList.Contains(p.Id))).ToListAsync(cancellationToken);
            var beachVTeams = await _dbContext.BeachVolleyballTeams.AsNoTracking().Include(t => t.Event).Include(t => t.Players).Where(t => eligibleEventIdsList.Contains(t.EventId) && t.Players!.Any(p => eligiblePlayerIdsList.Contains(p.Id))).ToListAsync(cancellationToken);
            var ttTeams = await _dbContext.TableTennisTeams.AsNoTracking().Include(t => t.Event).Include(t => t.Players).Where(t => eligibleEventIdsList.Contains(t.EventId) && t.Players!.Any(p => eligiblePlayerIdsList.Contains(p.Id))).ToListAsync(cancellationToken);
            var badTeams = await _dbContext.BadmintonTeams.AsNoTracking().Include(t => t.Event).Where(t => t.EventId != null && eligibleEventIdsList.Contains(t.EventId.Value) && ((t.Player1Id != null && eligiblePlayerIdsList.Contains(t.Player1Id.Value)) || (t.Player2Id != null && eligiblePlayerIdsList.Contains(t.Player2Id.Value)))).ToListAsync(cancellationToken);

            // 3. Match Stats
            var cwS = await _dbContext.ChessGames.AsNoTracking().Where(g => eligibleEventIdsList.Contains(g.TournamentId) && g.WhitePlayerId.HasValue && eligiblePlayerIdsList.Contains(g.WhitePlayerId.Value)).GroupBy(g => g.WhitePlayerId!.Value).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.Result == GameResult.WhiteWin) }).ToListAsync(cancellationToken);
            var cbS = await _dbContext.ChessGames.AsNoTracking().Where(g => eligibleEventIdsList.Contains(g.TournamentId) && g.BlackPlayerId.HasValue && eligiblePlayerIdsList.Contains(g.BlackPlayerId.Value)).GroupBy(g => g.BlackPlayerId!.Value).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.Result == GameResult.BlackWin) }).ToListAsync(cancellationToken);
            var chwS = await _dbContext.CheckersGames.AsNoTracking().Where(g => eligibleEventIdsList.Contains(g.TournamentId) && g.WhitePlayerId.HasValue && eligiblePlayerIdsList.Contains(g.WhitePlayerId.Value)).GroupBy(g => g.WhitePlayerId!.Value).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.Result == GameResult.WhiteWin) }).ToListAsync(cancellationToken);
            var chbS = await _dbContext.CheckersGames.AsNoTracking().Where(g => eligibleEventIdsList.Contains(g.TournamentId) && g.BlackPlayerId.HasValue && eligiblePlayerIdsList.Contains(g.BlackPlayerId.Value)).GroupBy(g => g.BlackPlayerId!.Value).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.Result == GameResult.BlackWin) }).ToListAsync(cancellationToken);
            var tt1S = await _dbContext.TableTennisMatches.AsNoTracking().Where(g => eligibleEventIdsList.Contains(g.TournamentId) && g.Player1Id.HasValue && eligiblePlayerIdsList.Contains(g.Player1Id.Value)).GroupBy(g => g.Player1Id!.Value).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.WinnerId == g.Key) }).ToListAsync(cancellationToken);
            var tt2S = await _dbContext.TableTennisMatches.AsNoTracking().Where(g => eligibleEventIdsList.Contains(g.TournamentId) && g.Player2Id.HasValue && eligiblePlayerIdsList.Contains(g.Player2Id.Value)).GroupBy(g => g.Player2Id!.Value).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.WinnerId == g.Key) }).ToListAsync(cancellationToken);
            var arm1S = await _dbContext.ArmwrestlingMatches.AsNoTracking().Where(g => eligibleEventIdsList.Contains(g.EventId) && eligiblePlayerIdsList.Contains(g.Player1Id)).GroupBy(g => g.Player1Id).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.WinnerId == g.Key) }).ToListAsync(cancellationToken);
            var arm2S = await _dbContext.ArmwrestlingMatches.AsNoTracking().Where(g => eligibleEventIdsList.Contains(g.EventId) && eligiblePlayerIdsList.Contains(g.Player2Id)).GroupBy(g => g.Player2Id).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.WinnerId == g.Key) }).ToListAsync(cancellationToken);
            var bad1S = await _dbContext.BadmintonMatches.AsNoTracking().Where(g => eligibleEventIdsList.Contains(g.EventId) && g.Player1Id != null && eligiblePlayerIdsList.Contains(g.Player1Id.Value)).GroupBy(g => g.Player1Id).Select(g => new { Id = g.Key!.Value, Count = g.Count(), Wins = g.Count(x => x.WinnerPlayerId == g.Key) }).ToListAsync(cancellationToken);
            var bad2S = await _dbContext.BadmintonMatches.AsNoTracking().Where(g => eligibleEventIdsList.Contains(g.EventId) && g.Player2Id != null && eligiblePlayerIdsList.Contains(g.Player2Id.Value)).GroupBy(g => g.Player2Id).Select(g => new { Id = g.Key!.Value, Count = g.Count(), Wins = g.Count(x => x.WinnerPlayerId == g.Key) }).ToListAsync(cancellationToken);

            var goalsSQuery = _dbContext.FootballGoals.AsNoTracking();
            if (seasonId != null)
            {
                goalsSQuery = goalsSQuery.Where(g => g.Match != null && eligibleEventIdsList.Contains(g.Match.EventId));
            }

            var goalsS = await goalsSQuery
                .Where(g => eligiblePlayerIdsList.Contains(g.PlayerId))
                .GroupBy(g => g.PlayerId)
                .Select(g => new { Id = g.Key, Goals = g.Sum(x => x.GoalsCount) })
                .ToListAsync(cancellationToken);
            
            var cardsSQuery = _dbContext.FootballCards.AsNoTracking();
            if (seasonId != null)
            {
                cardsSQuery = cardsSQuery.Where(c => c.Match != null && eligibleEventIdsList.Contains(c.Match.EventId));
            }
            
            var cardsS = await cardsSQuery
                .Where(c => eligiblePlayerIdsList.Contains(c.PlayerId))
                .Select(c => new { c.PlayerId, c.Type })
                .ToListAsync(cancellationToken);

            // Optimized Team Sports Match Stats loading
            var FootballHomeStats = await _dbContext.FootballMatches.AsNoTracking().Where(m => eligibleEventIdsList.Contains(m.EventId))
                .SelectMany(m => m.HomeRoster!.Where(p => eligiblePlayerIdsList.Contains(p.Id)).Select(p => new { PlayerId = p.Id, IsWinner = m.WinnerTeamId == m.HomeTeamId || (m.HomeTeamScore > m.AwayTeamScore && !m.WinnerTeamId.HasValue) }))
                .GroupBy(x => x.PlayerId).Select(g => new { Id = g.Key, Played = g.Count(), Wins = g.Count(x => x.IsWinner) }).ToListAsync(cancellationToken);
            var FootballAwayStats = await _dbContext.FootballMatches.AsNoTracking().Where(m => eligibleEventIdsList.Contains(m.EventId))
                .SelectMany(m => m.AwayRoster!.Where(p => eligiblePlayerIdsList.Contains(p.Id)).Select(p => new { PlayerId = p.Id, IsWinner = m.WinnerTeamId == m.AwayTeamId || (m.AwayTeamScore > m.HomeTeamScore && !m.WinnerTeamId.HasValue) }))
                .GroupBy(x => x.PlayerId).Select(g => new { Id = g.Key, Played = g.Count(), Wins = g.Count(x => x.IsWinner) }).ToListAsync(cancellationToken);

            var volleyHomeStats = await _dbContext.VolleyballMatches.AsNoTracking().Where(m => eligibleEventIdsList.Contains(m.EventId))
                .SelectMany(m => m.HomeRoster!.Where(p => eligiblePlayerIdsList.Contains(p.Id)).Select(p => new { PlayerId = p.Id, IsWinner = m.WinnerTeamId == m.HomeTeamId || (m.HomeSetsWon > m.AwaySetsWon && !m.WinnerTeamId.HasValue) }))
                .GroupBy(x => x.PlayerId).Select(g => new { Id = g.Key, Played = g.Count(), Wins = g.Count(x => x.IsWinner) }).ToListAsync(cancellationToken);
            var volleyAwayStats = await _dbContext.VolleyballMatches.AsNoTracking().Where(m => eligibleEventIdsList.Contains(m.EventId))
                .SelectMany(m => m.AwayRoster!.Where(p => eligiblePlayerIdsList.Contains(p.Id)).Select(p => new { PlayerId = p.Id, IsWinner = m.WinnerTeamId == m.AwayTeamId || (m.AwaySetsWon > m.HomeSetsWon && !m.WinnerTeamId.HasValue) }))
                .GroupBy(x => x.PlayerId).Select(g => new { Id = g.Key, Played = g.Count(), Wins = g.Count(x => x.IsWinner) }).ToListAsync(cancellationToken);

            var bballHomeStats = await _dbContext.Basketball3x3Matches.AsNoTracking().Where(m => eligibleEventIdsList.Contains(m.EventId))
                .SelectMany(m => m.HomeRoster!.Where(p => eligiblePlayerIdsList.Contains(p.Id)).Select(p => new { PlayerId = p.Id, IsWinner = m.WinnerTeamId == m.HomeTeamId || (m.HomeTeamScore > m.AwayTeamScore && !m.WinnerTeamId.HasValue) }))
                .GroupBy(x => x.PlayerId).Select(g => new { Id = g.Key, Played = g.Count(), Wins = g.Count(x => x.IsWinner) }).ToListAsync(cancellationToken);
            var bballAwayStats = await _dbContext.Basketball3x3Matches.AsNoTracking().Where(m => eligibleEventIdsList.Contains(m.EventId))
                .SelectMany(m => m.AwayRoster!.Where(p => eligiblePlayerIdsList.Contains(p.Id)).Select(p => new { PlayerId = p.Id, IsWinner = m.WinnerTeamId == m.AwayTeamId || (m.AwayTeamScore > m.HomeTeamScore && !m.WinnerTeamId.HasValue) }))
                .GroupBy(x => x.PlayerId).Select(g => new { Id = g.Key, Played = g.Count(), Wins = g.Count(x => x.IsWinner) }).ToListAsync(cancellationToken);

            var beachHomeStats = await _dbContext.BeachVolleyballMatches.AsNoTracking().Where(m => eligibleEventIdsList.Contains(m.EventId))
                .SelectMany(m => m.HomeRoster!.Where(p => eligiblePlayerIdsList.Contains(p.Id)).Select(p => new { PlayerId = p.Id, IsWinner = m.WinnerTeamId == m.HomeTeamId || (m.HomeSetsWon > m.AwaySetsWon && !m.WinnerTeamId.HasValue) }))
                .GroupBy(x => x.PlayerId).Select(g => new { Id = g.Key, Played = g.Count(), Wins = g.Count(x => x.IsWinner) }).ToListAsync(cancellationToken);
            var beachAwayStats = await _dbContext.BeachVolleyballMatches.AsNoTracking().Where(m => eligibleEventIdsList.Contains(m.EventId))
                .SelectMany(m => m.AwayRoster!.Where(p => eligiblePlayerIdsList.Contains(p.Id)).Select(p => new { PlayerId = p.Id, IsWinner = m.WinnerTeamId == m.AwayTeamId || (m.AwaySetsWon > m.HomeSetsWon && !m.WinnerTeamId.HasValue) }))
                .GroupBy(x => x.PlayerId).Select(g => new { Id = g.Key, Played = g.Count(), Wins = g.Count(x => x.IsWinner) }).ToListAsync(cancellationToken);

            // Pre-process into Lookups for speed
            var chessLookup = chessAch.ToLookup(a => a.PlayerId);
            var checkersLookup = checkersAch.ToLookup(a => a.PlayerId);
            var ttLookup = ttAch.ToLookup(a => a.PlayerId);
            var badmintonLookup = badmintonAch.ToLookup(a => a.PlayerId);
            var armwLookup = armwAch.ToLookup(a => a.PlayerId);
            var winnersLookup = genericWinners
                .SelectMany(e => new[] { 
                    new { Pid = e.FirstPlaceId, Ev = e }, 
                    new { Pid = e.SecondPlaceId, Ev = e }, 
                    new { Pid = e.ThirdPlaceId, Ev = e }, 
                    new { Pid = e.MvpId, Ev = e } 
                })
                .Where(x => x.Pid.HasValue)
                .ToLookup(x => x.Pid!.Value, x => x.Ev);
            
            var fTeamsLookup = FootballTeams.ToLookup(t => t.Id); // This needs to be by player... wait.
            // Let's build a flat list of (PlayerId, Team/Event) for lookup
            var footballPlayerEvents = FootballTeams.SelectMany(t => t.Players!.Select(p => new { PlayerId = p.Id, Team = t })).ToLookup(x => x.PlayerId);
            var volleyballPlayerEvents = volleyballTeams.SelectMany(t => t.Players!.Select(p => new { PlayerId = p.Id, Team = t })).ToLookup(x => x.PlayerId);
            var basketballPlayerEvents = bballTeams.SelectMany(t => t.Players!.Select(p => new { PlayerId = p.Id, Team = t })).ToLookup(x => x.PlayerId);
            var beachVPlayerEvents = beachVTeams.SelectMany(t => t.Players!.Select(p => new { PlayerId = p.Id, Team = t })).ToLookup(x => x.PlayerId);
            var ttTeamsLookup = ttTeams.SelectMany(t => t.Players!.Select(p => new { PlayerId = p.Id, Team = t })).ToLookup(x => x.PlayerId);
            var badmintonTeamsLookup = badTeams.SelectMany(t => new[] { t.Player1Id, t.Player2Id }.Where(id => id.HasValue).Select(id => new { PlayerId = id!.Value, Team = t })).ToLookup(x => x.PlayerId);

            var fH = FootballHomeStats.GroupBy(x => x.Id).ToDictionary(g => g.Key, g => g.First()); 
            var fA = FootballAwayStats.GroupBy(x => x.Id).ToDictionary(g => g.Key, g => g.First());
            var vH = volleyHomeStats.GroupBy(x => x.Id).ToDictionary(g => g.Key, g => g.First()); 
            var vA = volleyAwayStats.GroupBy(x => x.Id).ToDictionary(g => g.Key, g => g.First());
            var bH = bballHomeStats.GroupBy(x => x.Id).ToDictionary(g => g.Key, g => g.First()); 
            var bA = bballAwayStats.GroupBy(x => x.Id).ToDictionary(g => g.Key, g => g.First());
            var bvH = beachHomeStats.GroupBy(x => x.Id).ToDictionary(g => g.Key, g => g.First()); 
            var bvA = beachAwayStats.GroupBy(x => x.Id).ToDictionary(g => g.Key, g => g.First());

            var goalsLookup = goalsS.GroupBy(x => x.Id).ToDictionary(g => g.Key, g => g.First().Goals);
            var cardsLookup = cardsS.ToLookup(x => x.PlayerId);

            var playerDtos = new List<PlayerDto>();
            foreach (var player in players)
            {
                var dto = player.ToDto();
                var armwrestlingWeightClasses = new HashSet<PolytechLeague.Domain.Entities.Armwrestling.Enums.ArmwrestlingWeightClass>();
                if (player.ArmwrestlingWeight.HasValue) {
                    var inferredClass = player.ArmwrestlingWeight.Value switch { <= 60 => ArmwrestlingWeightClass.Under60kg, <= 70 => ArmwrestlingWeightClass.Under70kg, <= 80 => ArmwrestlingWeightClass.Under80kg, <= 90 => ArmwrestlingWeightClass.Under90kg, _ => ArmwrestlingWeightClass.Plus90kg };
                    armwrestlingWeightClasses.Add(inferredClass);
                    if (!dto.ArmwrestlingPreferredWeightClass.HasValue) dto.ArmwrestlingPreferredWeightClass = inferredClass;
                }
                var playerId = player.Id;
                var achievements = new List<AchievementDto>();
                var sportTypes = new HashSet<string>();

                foreach (var a in chessLookup[playerId]) {
                    if (a.Tournament == null) continue;
                    achievements.Add(new AchievementDto { TournamentName = a.Tournament.Name, SportType = "chess", Date = a.Tournament.Date, Rank = a.Rank, BoardRank = a.BoardRank, TournamentId = a.TournamentId, Result = GetResultLabel(a.Rank) });
                    sportTypes.Add("chess");
                }
                foreach (var a in checkersLookup[playerId]) {
                    if (a.Event == null) continue;
                    achievements.Add(new AchievementDto { TournamentName = a.Event.Name, SportType = "checkers", Date = a.Event.Date, Rank = a.Rank, BoardRank = a.BoardRank, TournamentId = a.TournamentId, Result = GetResultLabel(a.Rank) });
                    sportTypes.Add("checkers");
                }
                foreach (var a in ttLookup[playerId]) {
                    if (a.Tournament == null) continue;
                    achievements.Add(new AchievementDto { TournamentName = a.Tournament.Name, SportType = "table_tennis", Date = a.Tournament.Date, Rank = a.Rank, TournamentId = a.TournamentId, Result = GetResultLabel(a.Rank) });
                    sportTypes.Add("table_tennis");
                }
                foreach (var a in badmintonLookup[playerId]) {
                    if (a.Event == null) continue;
                    achievements.Add(new AchievementDto { TournamentName = a.Event.Name, SportType = "badminton", Date = a.Event.Date, Rank = a.Rank, TournamentId = a.EventId, Result = GetResultLabel(a.Rank) });
                    sportTypes.Add("badminton");
                }
                foreach (var a in armwLookup[playerId]) {
                    if (a.Tournament == null) continue;
                    achievements.Add(new AchievementDto { TournamentName = a.Tournament.Name, SportType = "armwrestling", Date = a.Tournament.Date, Rank = a.Rank, TournamentId = a.TournamentId, Result = GetResultLabel(a.Rank) });
                    sportTypes.Add("armwrestling");
                    armwrestlingWeightClasses.Add(a.Tournament.WeightClass);
                    if (!dto.ArmwrestlingPreferredWeightClass.HasValue) dto.ArmwrestlingPreferredWeightClass = a.Tournament.WeightClass;
                }
                
                // Generic Winners
                foreach (var e in winnersLookup[playerId].Where(x => x.FirstPlaceId == playerId || x.SecondPlaceId == playerId || x.ThirdPlaceId == playerId)) {
                    int r = (e.FirstPlaceId == playerId) ? 1 : ((e.SecondPlaceId == playerId) ? 2 : 3);
                    string sportType = GetSportType(e);
                    if (sportType == "other") continue;
                    
                    var existing = achievements.FirstOrDefault(a => a.TournamentId == e.Id && a.SportType == sportType);
                    if (existing == null) {
                        achievements.Add(new AchievementDto { TournamentName = e.Name, SportType = sportType, Date = e.Date, Rank = r, TournamentId = e.Id, Result = GetResultLabel(r) });
                        sportTypes.Add(sportType);
                    } else if (existing.Rank == 0 && r > 0) {
                        existing.Rank = r; existing.Result = GetResultLabel(r);
                    }
                    if (e is ArmwrestlingEvent armwEvent) {
                        armwrestlingWeightClasses.Add(armwEvent.WeightClass);
                        if (!dto.ArmwrestlingPreferredWeightClass.HasValue) dto.ArmwrestlingPreferredWeightClass = armwEvent.WeightClass;
                    }
                }
                foreach (var e in winnersLookup[playerId].Where(x => x.MvpId == playerId)) {
                    string sportType = e switch { FootballEvent => "football", VolleyballEvent => "volleyball", Basketball3x3Event => "basketball_3x3", BeachVolleyballEvent => "beach_volleyball", _ => "other" };
                    if (sportType == "other") continue;
                    if (!achievements.Any(a => a.TournamentId == e.Id && a.Result == "MVP")) {
                        achievements.Add(new AchievementDto { TournamentName = e.Name, SportType = sportType, Date = e.Date, Rank = 0, TournamentId = e.Id, Result = "MVP" });
                        sportTypes.Add(sportType);
                        dto.MvpCount++;
                    }
                }

                // Team Sports
                foreach (var x in footballPlayerEvents[playerId]) {
                    int r = (x.Team.Id == x.Team.Event!.FirstPlaceTeamId) ? 1 : ((x.Team.Id == x.Team.Event.SecondPlaceTeamId) ? 2 : ((x.Team.Id == x.Team.Event.ThirdPlaceTeamId) ? 3 : 0));
                    achievements.Add(new AchievementDto { TournamentName = x.Team.Event.Name, SportType = "football", Date = x.Team.Event.Date, Rank = r, TournamentId = x.Team.EventId, Result = GetResultLabel(r), IsCaptain = x.Team.CaptainId == playerId });
                    sportTypes.Add("football");
                }
                foreach (var x in volleyballPlayerEvents[playerId]) {
                    int r = (x.Team.Id == x.Team.Event!.FirstPlaceTeamId) ? 1 : ((x.Team.Id == x.Team.Event.SecondPlaceTeamId) ? 2 : ((x.Team.Id == x.Team.Event.ThirdPlaceTeamId) ? 3 : 0));
                    achievements.Add(new AchievementDto { TournamentName = x.Team.Event.Name, SportType = "volleyball", Date = x.Team.Event.Date, Rank = r, TournamentId = x.Team.EventId, Result = GetResultLabel(r) });
                    sportTypes.Add("volleyball");
                }
                foreach (var x in basketballPlayerEvents[playerId]) {
                    int r = (x.Team.Id == x.Team.Event!.FirstPlaceTeamId) ? 1 : ((x.Team.Id == x.Team.Event.SecondPlaceTeamId) ? 2 : ((x.Team.Id == x.Team.Event.ThirdPlaceTeamId) ? 3 : 0));
                    achievements.Add(new AchievementDto { TournamentName = x.Team.Event.Name, SportType = "basketball_3x3", Date = x.Team.Event.Date, Rank = r, TournamentId = x.Team.EventId, Result = GetResultLabel(r) });
                    sportTypes.Add("basketball_3x3");
                }
                foreach (var x in beachVPlayerEvents[playerId]) {
                    int r = (x.Team.Id == x.Team.Event!.FirstPlaceTeamId) ? 1 : ((x.Team.Id == x.Team.Event.SecondPlaceTeamId) ? 2 : ((x.Team.Id == x.Team.Event.ThirdPlaceTeamId) ? 3 : 0));
                    achievements.Add(new AchievementDto { TournamentName = x.Team.Event.Name, SportType = "beach_volleyball", Date = x.Team.Event.Date, Rank = r, TournamentId = x.Team.EventId, Result = GetResultLabel(r) });
                    sportTypes.Add("beach_volleyball");
                }
                foreach (var x in ttTeamsLookup[playerId]) {
                    int r = (x.Team.Id == x.Team.Event!.FirstPlaceTeamId) ? 1 : ((x.Team.Id == x.Team.Event.SecondPlaceTeamId) ? 2 : ((x.Team.Id == x.Team.Event.ThirdPlaceTeamId) ? 3 : 0));
                    achievements.Add(new AchievementDto { TournamentName = x.Team.Event.Name, SportType = "table_tennis", Date = x.Team.Event.Date, Rank = r, TournamentId = x.Team.EventId, Result = GetResultLabel(r) });
                    sportTypes.Add("table_tennis");
                }
                foreach (var x in badmintonTeamsLookup[playerId]) {
                    if (!x.Team.EventId.HasValue) continue;
                    int r = (x.Team.Id == x.Team.Event!.FirstPlaceTeamId) ? 1 : ((x.Team.Id == x.Team.Event.SecondPlaceTeamId) ? 2 : ((x.Team.Id == x.Team.Event.ThirdPlaceTeamId) ? 3 : 0));
                    achievements.Add(new AchievementDto { TournamentName = x.Team.Event.Name, SportType = "badminton", Date = x.Team.Event.Date, Rank = r, TournamentId = x.Team.EventId.Value, Result = GetResultLabel(r) });
                    sportTypes.Add("badminton");
                }

                dto.Achievements = achievements.OrderByDescending(a => a.Date).ToList();
                dto.Sports = sportTypes.ToList();
                dto.ArmwrestlingWeightClasses = armwrestlingWeightClasses.ToList();
                dto.CompetitionsCount = achievements.Select(a => a.TournamentId).Distinct().Count();
                
                // Score Calculation
                int played = 0, won = 0;
                double totalScore = 0;
                var sportStatsMap = new Dictionary<string, SportStatsDto>(StringComparer.OrdinalIgnoreCase);
                var pointsBreakdown = new List<PointEntryDto>();
                foreach (var sport in sportTypes) sportStatsMap[sport] = new SportStatsDto { SportType = sport };
                foreach (var ach in achievements) {
                    double pts = 0; double mult = 1.0; string levelName = "Міжгрупові";
                    if (allEvents.TryGetValue(ach.TournamentId, out var lvl)) {
                        if (levelMultipliers.TryGetValue(lvl, out var m)) mult = m;
                        levelName = lvl switch { CompetitionLevel.Regional => "Обласні", CompetitionLevel.National => "Всеукраїнські", _ => "Міжгрупові" };
                        pts = 10;
                    } else pts = 10;
                    if (ach.Rank == 1) pts += 100; else if (ach.Rank == 2) pts += 50; else if (ach.Rank == 3) pts += 30;
                    if (allEvents.TryGetValue(ach.TournamentId, out var eventLvl) && eventLvl != CompetitionLevel.CollegeGroup) {
                        if (ach.BoardRank == 1) pts += 40; else if (ach.BoardRank == 2) pts += 25; else if (ach.BoardRank == 3) pts += 15;
                    }
                    if (ach.Result == "MVP") pts += 15;
                    if (pts <= 0) continue;
                    var finalPts = (int)Math.Round(pts * mult);
                    totalScore += finalPts;
                    if (sportStatsMap.TryGetValue(ach.SportType, out var s)) s.PerformanceScore += finalPts;

                    string desc = ach.Result == "MVP" ? $"MVP: {ach.TournamentName} ({levelName})" : (ach.Rank > 0 ? $"{ach.Rank} місце: {ach.TournamentName} ({levelName})" : $"Участь: {ach.TournamentName} ({levelName})");
                    if (ach.BoardRank.HasValue && ach.BoardRank.Value > 0) {
                        desc += $", {ach.BoardRank.Value} місце на дошці";
                    }
                    pointsBreakdown.Add(new PointEntryDto { Description = desc, Points = finalPts, Category = "Achievements", SportType = ach.SportType });
                }

                void AddStats(string sport, int p, int w) {
                    played += p; won += w;
                    if (p > 0) {
                        sportTypes.Add(sport);
                        if (!sportStatsMap.ContainsKey(sport)) sportStatsMap[sport] = new SportStatsDto { SportType = sport };
                        var s = sportStatsMap[sport];
                        s.MatchesPlayed += p; s.WinsCount += w;
                    }
                }

                var cw = cwS.FirstOrDefault(x => x.Id == playerId); var cb = cbS.FirstOrDefault(x => x.Id == playerId);
                var chw = chwS.FirstOrDefault(x => x.Id == playerId); var chb = chbS.FirstOrDefault(x => x.Id == playerId);
                var t1 = tt1S.FirstOrDefault(x => x.Id == playerId); var t2 = tt2S.FirstOrDefault(x => x.Id == playerId);
                var a1 = arm1S.FirstOrDefault(x => x.Id == playerId); var a2 = arm2S.FirstOrDefault(x => x.Id == playerId);
                var b1 = bad1S.FirstOrDefault(x => x.Id == playerId); var b2 = bad2S.FirstOrDefault(x => x.Id == playerId);
                
                AddStats("chess", (cw?.Count ?? 0) + (cb?.Count ?? 0), (cw?.Wins ?? 0) + (cb?.Wins ?? 0));
                AddStats("checkers", (chw?.Count ?? 0) + (chb?.Count ?? 0), (chw?.Wins ?? 0) + (chb?.Wins ?? 0));
                AddStats("table_tennis", (t1?.Count ?? 0) + (t2?.Count ?? 0), (t1?.Wins ?? 0) + (t2?.Wins ?? 0));
                AddStats("armwrestling", (a1?.Count ?? 0) + (a2?.Count ?? 0), (a1?.Wins ?? 0) + (a2?.Wins ?? 0));
                AddStats("badminton", (b1?.Count ?? 0) + (b2?.Count ?? 0), (b1?.Wins ?? 0) + (b2?.Wins ?? 0));
                
                var fPlayed = (fH.TryGetValue(playerId, out var fh) ? fh.Played : 0) + (fA.TryGetValue(playerId, out var fa) ? fa.Played : 0);
                var fWins = (fH.TryGetValue(playerId, out var fhw) ? fhw.Wins : 0) + (fA.TryGetValue(playerId, out var faw) ? faw.Wins : 0);
                AddStats("football", fPlayed, fWins);
                var vPlayed = (vH.TryGetValue(playerId, out var vh) ? vh.Played : 0) + (vA.TryGetValue(playerId, out var va) ? va.Played : 0);
                var vWins = (vH.TryGetValue(playerId, out var vhw) ? vhw.Wins : 0) + (vA.TryGetValue(playerId, out var vaw) ? vaw.Wins : 0);
                AddStats("volleyball", vPlayed, vWins);
                var bPlayed = (bH.TryGetValue(playerId, out var bh) ? bh.Played : 0) + (bA.TryGetValue(playerId, out var ba) ? ba.Played : 0);
                var bWins = (bH.TryGetValue(playerId, out var bhw) ? bhw.Wins : 0) + (bA.TryGetValue(playerId, out var baw) ? baw.Wins : 0);
                AddStats("basketball_3x3", bPlayed, bWins);
                var bvPlayed = (bvH.TryGetValue(playerId, out var bvh) ? bvh.Played : 0) + (bvA.TryGetValue(playerId, out var bva) ? bva.Played : 0);
                var bvWins = (bvH.TryGetValue(playerId, out var bvhw) ? bvhw.Wins : 0) + (bvA.TryGetValue(playerId, out var bvaw) ? bvaw.Wins : 0);
                AddStats("beach_volleyball", bvPlayed, bvWins);

                if (fPlayed > 0) {
                    var s = sportStatsMap["football"];
                    int winPts = fWins * 10; int playPts = fPlayed * 5;
                    s.PerformanceScore += winPts + playPts; totalScore += winPts + playPts;
                    pointsBreakdown.Add(new PointEntryDto { Description = $"Участь (Футбол): {fPlayed} ігор", Points = playPts, Category = "Participation", SportType = "football" });
                    if (fWins > 0) pointsBreakdown.Add(new PointEntryDto { Description = $"Перемоги (Футбол): {fWins}", Points = winPts, Category = "Wins", SportType = "football" });
                }
                if (goalsLookup.TryGetValue(playerId, out var pGoals)) {
                    var s = sportStatsMap["football"]; s.GoalsCount += pGoals;
                    int goalPts = pGoals * 5; s.PerformanceScore += goalPts; totalScore += goalPts;
                    pointsBreakdown.Add(new PointEntryDto { Description = $"Голи (Футбол): {pGoals}", Points = goalPts, Category = "Goals", SportType = "football" });
                    dto.GoalsCount = pGoals;
                }
                var pCards = cardsLookup[playerId].ToList();
                int yellow = pCards.Count(x => x.Type == FootballCardType.Yellow);
                int red = pCards.Count(x => x.Type == FootballCardType.Red);
                if (yellow > 0 || red > 0) {
                    var s = sportStatsMap["football"]; s.YellowCardsCount = yellow; s.RedCardsCount = red;
                    dto.YellowCardsCount = yellow; dto.RedCardsCount = red;
                }

                dto.MatchesPlayed = played; dto.WinsCount = won;
                dto.PerformanceScore = (int)Math.Round(totalScore);
                dto.SportStats = sportStatsMap.Values.ToList();
                dto.PointsBreakdown = pointsBreakdown.OrderByDescending(x => x.Points).ToList();
                
                if (achievements.Count > 0 || played > 0 || dto.GoalsCount > 0 || yellow > 0 || red > 0) {
                    playerDtos.Add(dto);
                }
            }
            var allRanked = playerDtos.OrderByDescending(p => p.PerformanceScore).ToList();
            for (int i = 0; i < allRanked.Count; i++) allRanked[i].GlobalRank = i + 1;
            var allSportTypes = playerDtos.SelectMany(p => p.SportStats.Select(s => s.SportType)).Distinct().ToList();
            foreach (var sport in allSportTypes) {
                var rankedForSport = playerDtos.Where(p => p.SportStats.Any(s => s.SportType == sport)).OrderByDescending(p => p.SportStats.First(s => s.SportType == sport).PerformanceScore).ToList();
                for (int i = 0; i < rankedForSport.Count; i++) rankedForSport[i].SportStats.First(s => s.SportType == sport).Rank = i + 1;
            }
            return allRanked;
        } catch (Exception ex) {
            _logger.LogError(ex, "Error in GetAllWithRankingsAsync: {Message}", ex.Message);
            throw;
        }
    }

    private async Task PopulateAchievementsAsync(PlayerDto dto, CancellationToken cancellationToken)
    {
        if (dto.Id == null) return;
        var playerId = dto.Id.Value;
        var achievements = new List<AchievementDto>();
        // 1. Chess
        try {
            var chessPerformances = await _dbContext.ChessEventParticipants
                .Include(p => p.Tournament)
                .Where(p => p.PlayerId == playerId)
                .ToListAsync(cancellationToken);
            foreach (var p in chessPerformances) {
                if (p.Tournament == null) continue;
                int rank = p.Rank;
                if (rank <= 0) {
                    if (p.Tournament.FirstPlaceId == playerId) rank = 1;
                    else if (p.Tournament.SecondPlaceId == playerId) rank = 2;
                    else if (p.Tournament.ThirdPlaceId == playerId) rank = 3;
                    else {
                        rank = await _dbContext.ChessEventParticipants
                            .Where(other => other.TournamentId == p.TournamentId && other.Points > p.Points)
                            .CountAsync(cancellationToken) + 1;
                    }
                }
                var resultStr = rank > 0 ? $"{rank} місце" : "Учасник";
                if (p.Points > 0) resultStr += $" ({p.Points} очок)";
                if (p.BoardRank.HasValue && p.BoardRank.Value > 0) {
                    resultStr += $", {p.BoardRank.Value} місце на дошці";
                }

                achievements.Add(new AchievementDto {
                    TournamentName = p.Tournament.Name,
                    SportType = "chess",
                    Date = p.Tournament.Date,
                    Rank = rank,
                    BoardRank = p.BoardRank,
                    TournamentId = p.TournamentId,
                    Result = resultStr
                });
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error populating Chess achievements for player {PlayerId}", playerId);
        }
        // 2. Checkers
        try {
            var checkersPerformances = await _dbContext.CheckersEventParticipants
                .Include(p => p.Event)
                .Where(p => p.PlayerId == playerId)
                .ToListAsync(cancellationToken);
            foreach (var p in checkersPerformances) {
                if (p.Event == null) continue;
                int rank = p.Rank;
                if (rank <= 0) {
                    if (p.Event.FirstPlaceId == playerId) rank = 1;
                    else if (p.Event.SecondPlaceId == playerId) rank = 2;
                    else if (p.Event.ThirdPlaceId == playerId) rank = 3;
                    else {
                        rank = await _dbContext.CheckersEventParticipants
                            .Where(other => other.TournamentId == p.TournamentId && other.Points > p.Points)
                            .CountAsync(cancellationToken) + 1;
                    }
                }
                var resultStr = rank > 0 ? $"{rank} місце" : "Учасник";
                if (p.Points > 0) resultStr += $" ({p.Points} очок)";
                if (p.BoardRank.HasValue && p.BoardRank.Value > 0) {
                    resultStr += $", {p.BoardRank.Value} місце на дошці";
                }

                achievements.Add(new AchievementDto {
                    TournamentName = p.Event.Name,
                    SportType = "checkers",
                    Date = p.Event.Date,
                    Rank = rank,
                    BoardRank = p.BoardRank,
                    TournamentId = p.TournamentId,
                    Result = resultStr
                });
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error populating Checkers achievements for player {PlayerId}", playerId);
        }
        // 3. Table Tennis
        try {
            var ttPerformances = await _dbContext.TableTennisEventParticipants
                .Include(p => p.Tournament)
                .Where(p => p.PlayerId == playerId)
                .ToListAsync(cancellationToken);
            foreach (var p in ttPerformances) {
                if (p.Tournament == null) continue;
                int rank = p.Rank;
                if (rank <= 0) {
                    if (p.Tournament.FirstPlaceId == playerId) rank = 1;
                    else if (p.Tournament.SecondPlaceId == playerId) rank = 2;
                    else if (p.Tournament.ThirdPlaceId == playerId) rank = 3;
                    else {
                        rank = await _dbContext.TableTennisEventParticipants
                            .Where(other => other.TournamentId == p.TournamentId && other.Points > p.Points)
                            .CountAsync(cancellationToken) + 1;
                    }
                }
                achievements.Add(new AchievementDto {
                    TournamentName = p.Tournament.Name,
                    SportType = "table_tennis",
                    Date = p.Tournament.Date,
                    Rank = rank,
                    TournamentId = p.TournamentId,
                    Result = rank > 0 ? $"{rank} місце" : "Учасник"
                });
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error populating Table Tennis achievements for player {PlayerId}", playerId);
        }
        // 4. Badminton
        try {
            var badmintonPerformances = await _dbContext.BadmintonEventParticipants
                .Include(p => p.Event)
                .Where(p => p.PlayerId == playerId)
                .ToListAsync(cancellationToken);
            foreach (var p in badmintonPerformances) {
                if (p.Event == null) continue;
                int rank = p.Rank;
                if (rank <= 0) {
                    if (p.Event.FirstPlaceId == playerId) rank = 1;
                    else if (p.Event.SecondPlaceId == playerId) rank = 2;
                    else if (p.Event.ThirdPlaceId == playerId) rank = 3;
                    else {
                        rank = await _dbContext.BadmintonEventParticipants
                            .Where(other => other.EventId == p.EventId && other.Points > p.Points)
                            .CountAsync(cancellationToken) + 1;
                    }
                }
                achievements.Add(new AchievementDto {
                    TournamentName = p.Event.Name,
                    SportType = "badminton",
                    Date = p.Event.Date,
                    Rank = rank,
                    TournamentId = p.EventId,
                    Result = rank > 0 ? $"{rank} місце" : "Учасник"
                });
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error populating Badminton achievements for player {PlayerId}", playerId);
        }
        // 5. Armwrestling & Generic Individual Winners
        try {
            var armwPerformances = await _dbContext.ArmwrestlingEventParticipants
                .Include(p => p.Tournament)
                .Where(p => p.PlayerId == playerId)
                .ToListAsync(cancellationToken);
            foreach (var p in armwPerformances) {
                if (p.Tournament == null) continue;
                int rank = p.Rank;
                achievements.Add(new AchievementDto {
                    TournamentName = p.Tournament.Name,
                    SportType = "armwrestling",
                    Date = p.Tournament.Date,
                    Rank = rank,
                    TournamentId = p.TournamentId,
                    Result = rank > 0 ? $"{rank} місце" : "Учасник"
                });
            }
            var genericWinners = await _dbContext.Set<Event>()
                .Where(e => e.FirstPlaceId == playerId || e.SecondPlaceId == playerId || e.ThirdPlaceId == playerId)
                .ToListAsync(cancellationToken);
            foreach (var e in genericWinners) {
                string sport = GetSportType(e);
                if (achievements.Any(a => a.TournamentId == e.Id && (a.SportType == sport || a.SportType == "individual"))) continue;
                int rank = e.FirstPlaceId == playerId ? 1 : (e.SecondPlaceId == playerId ? 2 : 3);
                achievements.Add(new AchievementDto {
                    TournamentName = e.Name,
                    SportType = sport,
                    Date = e.Date,
                    Rank = rank,
                    TournamentId = e.Id,
                    Result = GetResultLabel(rank)
                });
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error populating Generic achievements for player {PlayerId}", playerId);
        }
        // 6. Team Sports (Football, Volleyball, etc.)
        // Football
        try {
            var teams = await _dbContext.FootballTeams
                .Include(t => t.Event)
                .Where(t => t.Players != null && t.Players.Any(pl => pl.Id == playerId))
                .ToListAsync(cancellationToken);
            foreach (var team in teams) {
                if (team.Event == null) continue;
                int rank = GetTeamRank(team.Event, team.Id);
                if (rank == 0 && team.Points > 0) {
                    rank = await _dbContext.FootballTeams
                        .Where(others => others.EventId == team.EventId && others.Points > team.Points)
                        .CountAsync(cancellationToken) + 1;
                }
                achievements.Add(new AchievementDto {
                    TournamentName = team.Event.Name,
                    SportType = "football",
                    Date = team.Event.Date,
                    Rank = rank,
                    TournamentId = team.EventId,
                    IsCaptain = team.CaptainId == playerId,
                    Result = rank > 0 ? $"{rank} місце (команда {team.Name})" : $"Учасник (команда {team.Name})"
                });
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error populating Football achievements for player {PlayerId}", playerId);
        }
        // Other Team Sports (Volleyball, Basketball, Beach Volleyball)
        try {
            // Volleyball
            var vTeams = await _dbContext.VolleyballTeams
                .Include(t => t.Event)
                .Where(t => t.Players != null && t.Players.Any(pl => pl.Id == playerId))
                .ToListAsync(cancellationToken);
            foreach (var t in vTeams) {
                if (t.Event == null) continue;
                int rank = GetTeamRank(t.Event, t.Id);
                achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "volleyball", Date = t.Event.Date, Rank = rank, TournamentId = t.EventId, Result = rank > 0 ? $"{rank} місце ({t.Name})" : $"Учасник ({t.Name})" });
            }
            // Basketball 3x3
            var bTeams = await _dbContext.Basketball3x3Teams
                .Include(t => t.Event)
                .Where(t => t.Players != null && t.Players.Any(pl => pl.Id == playerId))
                .ToListAsync(cancellationToken);
            foreach (var t in bTeams) {
                if (t.Event == null) continue;
                int rank = GetTeamRank(t.Event, t.Id);
                achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "basketball_3x3", Date = t.Event.Date, Rank = rank, TournamentId = t.EventId, Result = rank > 0 ? $"{rank} місце ({t.Name})" : $"Учасник ({t.Name})" });
            }
            // Beach Volleyball
            var bvTeams = await _dbContext.BeachVolleyballTeams
                .Include(t => t.Event)
                .Where(t => t.Players != null && t.Players.Any(pl => pl.Id == playerId))
                .ToListAsync(cancellationToken);
            foreach (var t in bvTeams) {
                if (t.Event == null) continue;
                int rank = GetTeamRank(t.Event, t.Id);
                achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "beach_volleyball", Date = t.Event.Date, Rank = rank, TournamentId = t.EventId, Result = rank > 0 ? $"{rank} місце ({t.Name})" : $"Учасник ({t.Name})" });
            }
            // Table Tennis Teams
            var ttTeams = await _dbContext.TableTennisTeams
                .Include(t => t.Event)
                .Where(t => t.Players.Any(pl => pl.Id == playerId))
                .ToListAsync(cancellationToken);
            foreach (var t in ttTeams) {
                if (t.Event == null) continue;
                int rank = (t.Id == t.Event.FirstPlaceTeamId) ? 1 : ((t.Id == t.Event.SecondPlaceTeamId) ? 2 : ((t.Id == t.Event.ThirdPlaceTeamId) ? 3 : 0));
                achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "table_tennis", Date = t.Event.Date, Rank = rank, TournamentId = t.EventId, Result = rank > 0 ? $"{rank} місце ({t.Name})" : $"Учасник ({t.Name})" });
            }
            // Badminton Teams
            var badTeams = await _dbContext.BadmintonTeams
                .Include(t => t.Event)
                .Where(t => t.Player1Id == playerId || t.Player2Id == playerId)
                .ToListAsync(cancellationToken);
            foreach (var t in badTeams) {
                if (t.Event == null || !t.EventId.HasValue) continue;
                int rank = (t.Id == t.Event.FirstPlaceTeamId) ? 1 : ((t.Id == t.Event.SecondPlaceTeamId) ? 2 : ((t.Id == t.Event.ThirdPlaceTeamId) ? 3 : 0));
                achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "badminton", Date = t.Event.Date, Rank = rank, TournamentId = t.EventId.Value, Result = rank > 0 ? $"{rank} місце ({t.Name})" : $"Учасник ({t.Name})" });
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error populating other team achievements for player {PlayerId}", playerId);
        }

        // Athletics
        try {
            var athleticsResults = await _dbContext.AthleticsResults
                .Include(r => r.AthleticsEvent)
                .Where(r => r.PlayerId == playerId)
                .ToListAsync(cancellationToken);
            foreach (var r in athleticsResults) {
                if (r.AthleticsEvent == null) continue;
                achievements.Add(new AchievementDto {
                    TournamentName = $"{r.AthleticsEvent.Name} ({r.DisciplineName})",
                    SportType = "athletics",
                    Date = r.AthleticsEvent.Date,
                    Rank = r.Rank,
                    TournamentId = r.AthleticsEventId,
                    Result = $"{r.Rank} місце - {r.Score}"
                });
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error populating Athletics achievements for player {PlayerId}", playerId);
        }

        // 8. MVP Achievements
        try {
            var mvpEvents = await _dbContext.Set<Event>()
                .Where(e => e.MvpId == playerId)
                .ToListAsync(cancellationToken);
            foreach (var e in mvpEvents) {
                string sport = e switch {
                    FootballEvent => "football",
                    VolleyballEvent => "volleyball",
                    Basketball3x3Event => "basketball_3x3",
                    _ => "other"
                };
                if (sport == "other") continue;
                if (achievements.Any(a => a.TournamentId == e.Id && a.Result == "MVP")) continue;
                
                achievements.Add(new AchievementDto {
                    TournamentName = e.Name,
                    SportType = sport,
                    Date = e.Date,
                    Rank = 0,
                    TournamentId = e.Id,
                    Result = "MVP"
                });
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error populating MVP achievements for player {PlayerId}", playerId);
        }

        dto.Achievements = achievements.OrderByDescending(a => a.Date).ToList();
        dto.CompetitionsCount = achievements.Select(a => a.TournamentId).Distinct().Count();
    }

    private async Task PopulateStatsAsync(PlayerDto dto, CancellationToken cancellationToken)
    {
        if (dto.Id == null) return;
        var playerId = dto.Id.Value;
        var pointsBreakdown = new List<PointEntryDto>();
        double totalScore = 0;
        // 1. Achievements Points (from previously populated Achievements list)
        var levelMultipliers = new Dictionary<CompetitionLevel, double> {
            { CompetitionLevel.CollegeGroup, 1.0 },
            { CompetitionLevel.Regional, 2.5 },
            { CompetitionLevel.National, 5.0 }
        };
        var eventIds = dto.Achievements.Select(a => a.TournamentId).Distinct().ToList();
        var eventLevels = await _dbContext.Set<Event>()
            .Where(e => eventIds.Contains(e.Id))
            .Select(e => new { e.Id, e.Level })
            .ToDictionaryAsync(e => e.Id, e => e.Level, cancellationToken);
        foreach (var ach in dto.Achievements) {
            double basePts = (ach.Rank == 1) ? 100 : (ach.Rank == 2 ? 50 : (ach.Rank == 3 ? 30 : 0));
            if (basePts <= 0) continue;
            double mult = 1.0;
            string levelName = "Міжгрупові";
            if (eventLevels.TryGetValue(ach.TournamentId, out var lvl)) {
                if (levelMultipliers.TryGetValue(lvl, out var m)) mult = m;
                levelName = lvl switch {
                    CompetitionLevel.Regional => "Обласні",
                    CompetitionLevel.National => "Всеукраїнські",
                    _ => "Міжгрупові"
                };
            }
            int finalPts = (int)Math.Round(basePts * mult);
            totalScore += finalPts;
            pointsBreakdown.Add(new PointEntryDto {
                Description = $"{ach.Rank} місце: {ach.TournamentName} ({levelName})",
                Points = finalPts,
                Category = "Achievements"
            });
        }
        foreach (var ach in dto.Achievements.Where(a => a.Result == "MVP")) {
            int mvpPts = 50;
            totalScore += mvpPts;
            pointsBreakdown.Add(new PointEntryDto {
                Description = $"MVP: {ach.TournamentName}",
                Points = mvpPts,
                Category = "MVP"
            });
        }

        // 2. Individual Matches Played & Wins
        int chessMatches = await _dbContext.ChessGames.CountAsync(g => g.WhitePlayerId == playerId || g.BlackPlayerId == playerId, cancellationToken);
        int chessWins = await _dbContext.ChessGames.CountAsync(g => (g.WhitePlayerId == playerId && g.Result == GameResult.WhiteWin) || (g.BlackPlayerId == playerId && g.Result == GameResult.BlackWin), cancellationToken);
        int checkersMatches = await _dbContext.CheckersGames.CountAsync(g => g.WhitePlayerId == playerId || g.BlackPlayerId == playerId, cancellationToken);
        int checkersWins = await _dbContext.CheckersGames.CountAsync(g => (g.WhitePlayerId == playerId && g.Result == GameResult.WhiteWin) || (g.BlackPlayerId == playerId && g.Result == GameResult.BlackWin), cancellationToken);
        int ttMatches = await _dbContext.TableTennisMatches.CountAsync(g => g.Player1Id == playerId || g.Player2Id == playerId, cancellationToken);
        int ttWins = await _dbContext.TableTennisMatches.CountAsync(g => g.WinnerId == playerId, cancellationToken);
        int armMatches = await _dbContext.ArmwrestlingMatches.CountAsync(g => g.Player1Id == playerId || g.Player2Id == playerId, cancellationToken);
        int armWins = await _dbContext.ArmwrestlingMatches.CountAsync(g => g.WinnerId == playerId, cancellationToken);
        int badMatches = await _dbContext.BadmintonMatches.CountAsync(g => g.Player1Id == playerId || g.Player2Id == playerId, cancellationToken);
        int badWins = await _dbContext.BadmintonMatches.CountAsync(g => g.WinnerPlayerId == playerId, cancellationToken);

        void AddActivityStats(string sportName, int p, int w) {
            if (p <= 0) return;
            int pts = (w * 2);
            totalScore += pts;
            if (w > 0) pointsBreakdown.Add(new PointEntryDto { Description = $"Перемоги ({sportName}): {w} ігор", Points = w * 2, Category = "Activity" });
        }
        AddActivityStats("Шахи", chessMatches, chessWins);
        AddActivityStats("Шашки", checkersMatches, checkersWins);
        AddActivityStats("Настільний теніс", ttMatches, ttWins);
        AddActivityStats("Армрестлінг", armMatches, armWins);
        AddActivityStats("Бадмінтон", badMatches, badWins);
        
        dto.MatchesPlayed = chessMatches + checkersMatches + ttMatches + armMatches + badMatches;
        dto.WinsCount = chessWins + checkersWins + ttWins + armWins + badWins;
        
        // 3. Football Goals
        int FootballGoals = await _dbContext.FootballGoals
            .Where(g => g.PlayerId == playerId)
            .SumAsync(g => (int?)g.GoalsCount ?? 0, cancellationToken);
        
        if (FootballGoals > 0) {
            int goalPts = FootballGoals * 5;
            totalScore += goalPts;
            pointsBreakdown.Add(new PointEntryDto {
                Description = $"Забиті голи (Футбол): {FootballGoals} голів",
                Points = goalPts,
                Category = "Goals"
            });
        }
        dto.GoalsCount = FootballGoals;
        
        // 4. Football Cards
        int yellowCards = await _dbContext.FootballCards
            .CountAsync(c => c.PlayerId == playerId && (int)c.Type == 1, cancellationToken);
        int redCards = await _dbContext.FootballCards
            .CountAsync(c => c.PlayerId == playerId && (int)c.Type == 2, cancellationToken);
            
        dto.YellowCardsCount = yellowCards;
        dto.RedCardsCount = redCards;

        dto.PerformanceScore = (int)Math.Round(totalScore);
        dto.PointsBreakdown = pointsBreakdown.OrderByDescending(x => x.Points).ToList();
    }

    private string GetSportType(Event e)
    {
        return e switch
        {
            ChessEvent => "chess",
            CheckersEvent => "checkers",
            FootballEvent => "football",
            VolleyballEvent => "volleyball",
            Basketball3x3Event => "basketball_3x3",
            BeachVolleyballEvent => "beach_volleyball",
            ArmwrestlingEvent => "armwrestling",
            TableTennisEvent => "table_tennis",
            BadmintonEvent => "badminton",
            AthleticsEvent => "athletics",
            _ => "other"
        };
    }

    private string GetResultLabel(int? rank)
    {
        if (rank == 1) return "1 місце";
        if (rank == 2) return "2 місце";
        if (rank == 3) return "3 місце";
        if (rank > 0) return $"{rank} місце";
        return "Учасник";
    }

    private int GetTeamRank(Event e, long teamId)
    {
        if (e is FootballEvent fe)
        {
            if (fe.FirstPlaceTeamId == teamId) return 1;
            if (fe.SecondPlaceTeamId == teamId) return 2;
            if (fe.ThirdPlaceTeamId == teamId) return 3;
        }
        else if (e is VolleyballEvent ve)
        {
            if (ve.FirstPlaceTeamId == teamId) return 1;
            if (ve.SecondPlaceTeamId == teamId) return 2;
            if (ve.ThirdPlaceTeamId == teamId) return 3;
        }
        else if (e is Basketball3x3Event be)
        {
            if (be.FirstPlaceTeamId == teamId) return 1;
            if (be.SecondPlaceTeamId == teamId) return 2;
            if (be.ThirdPlaceTeamId == teamId) return 3;
        }
        else if (e is BeachVolleyballEvent bve)
        {
            if (bve.FirstPlaceTeamId == teamId) return 1;
            if (bve.SecondPlaceTeamId == teamId) return 2;
            if (bve.ThirdPlaceTeamId == teamId) return 3;
        }
        else if (e is TableTennisEvent tte)
        {
            if (tte.FirstPlaceTeamId == teamId) return 1;
            if (tte.SecondPlaceTeamId == teamId) return 2;
            if (tte.ThirdPlaceTeamId == teamId) return 3;
        }

        return 0;
    }
    // Profile Merging Logic

    public async Task<IEnumerable<PlayerDto>> FindPotentialArchivalMatchesAsync(long userId, string firstName, string lastName, string group, CancellationToken cancellationToken = default)
    {
        var existingRequestTargetIds = await _dbContext.ProfileMergeRequests
            .Where(r => r.UserId == userId)
            .Select(r => r.TargetPlayerId)
            .ToListAsync(cancellationToken);

        var matches = await _dbContext.Players
            .Where(p => p.UserId == null &&
                        p.FirstName.ToLower() == firstName.ToLower() &&
                        p.LastName.ToLower() == lastName.ToLower() &&
                        !existingRequestTargetIds.Contains(p.Id))
            .ToListAsync(cancellationToken);
        return matches.Select(p => p.ToDto());
    }

    public async Task CreateMergeRequestAsync(long userId, long targetPlayerId, CancellationToken cancellationToken = default)
    {
        var existingRequest = await _dbContext.ProfileMergeRequests
            .FirstOrDefaultAsync(r => r.UserId == userId && r.TargetPlayerId == targetPlayerId && r.Status == RegistrationStatus.Pending, cancellationToken);
        if (existingRequest != null) return;
        var request = new ProfileMergeRequest
        {
            UserId = userId,
            TargetPlayerId = targetPlayerId,
            Status = RegistrationStatus.Pending
        };
        _dbContext.ProfileMergeRequests.Add(request);
        await _dbContext.SaveChangesAsync(cancellationToken);
        var user = await _dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        var player = await _dbContext.Players.FindAsync(new object[] { targetPlayerId }, cancellationToken);
        await _notificationService.SendNotificationToAdminsAsync(
            "Запит на об'єднання профілю",
            $"Користувач {user?.FirstName} {user?.LastName} стверджує, що він — це гравець {player?.FirstName} {player?.LastName} ({player?.Group}).",
            NotificationType.System,
            $"/admin/merges"
        );
    }

    public async Task<IEnumerable<ProfileMergeRequestDto>> GetPendingMergeRequestsAsync(CancellationToken cancellationToken = default)
    {
        var requests = await _dbContext.ProfileMergeRequests
            .Include(r => r.User)
            .Include(r => r.TargetPlayer)
            .Where(r => r.Status == RegistrationStatus.Pending)
            .ToListAsync(cancellationToken);
        return requests.Select(r => new ProfileMergeRequestDto
        {
            Id = r.Id,
            UserId = r.UserId,
            UserFullName = $"{r.User?.FirstName} {r.User?.LastName}",
            TargetPlayerId = r.TargetPlayerId,
            TargetPlayerName = $"{r.TargetPlayer?.FirstName} {r.TargetPlayer?.LastName}",
            TargetPlayerGroup = r.TargetPlayer?.Group,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            AdminComment = r.AdminComment
        });
    }

    public async Task ApproveMergeRequestAsync(long requestId, CancellationToken cancellationToken = default)
    {
        var request = await _dbContext.ProfileMergeRequests
            .Include(r => r.User)
            .Include(r => r.TargetPlayer)
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);
        if (request == null || request.Status != RegistrationStatus.Pending) return;
        var targetPlayer = request.TargetPlayer;
        if (targetPlayer == null) return;
        // 1. Check if user already has a Player profile (likely created automatically by GetProfile)
        var currentPlayer = await _dbContext.Players
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);
        if (currentPlayer != null && currentPlayer.Id != targetPlayer.Id)
        {
            // MOVE ALL DATA FROM currentPlayer TO targetPlayer
            await MergePlayersDataAsync(currentPlayer.Id, targetPlayer.Id, cancellationToken);
            // Delete the temporary current player
            _dbContext.Players.Remove(currentPlayer);
        }
        // 2. Link User to the Archival Player
        targetPlayer.UserId = request.UserId;
        request.Status = RegistrationStatus.Approved;
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _notificationService.SendNotificationToUserAsync(
            request.UserId,
            "Запит схвалено",
            "Ваш запит на об'єднання профілю було схвалено. Тепер ваша історія досягнень доступна у вашому аккаунті.",
            NotificationType.System
        );
    }

    public async Task RejectMergeRequestAsync(long requestId, string comment, CancellationToken cancellationToken = default)
    {
        var request = await _dbContext.ProfileMergeRequests.FindAsync(new object[] { requestId }, cancellationToken);
        if (request == null) return;
        request.Status = RegistrationStatus.Rejected;
        request.AdminComment = comment;
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _notificationService.SendNotificationToUserAsync(
            request.UserId,
            "Запит відхилено",
            $"Ваш запит на об'єднання профілю було відхилено. Причина: {comment}",
            NotificationType.System
        );
    }

    public async Task<IEnumerable<ProfileMergeRequestDto>> GetAllMergeRequestsAsync(CancellationToken cancellationToken = default)
    {
        var requests = await _dbContext.ProfileMergeRequests
            .Include(r => r.User)
            .Include(r => r.TargetPlayer)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return requests.Select(r => new ProfileMergeRequestDto
        {
            Id = r.Id,
            UserId = r.UserId,
            UserFullName = $"{r.User?.FirstName} {r.User?.LastName}",
            TargetPlayerId = r.TargetPlayerId,
            TargetPlayerName = $"{r.TargetPlayer?.FirstName} {r.TargetPlayer?.LastName}",
            TargetPlayerGroup = r.TargetPlayer?.Group,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            AdminComment = r.AdminComment
        });
    }

    public async Task IgnoreMergeRequestAsync(long userId, long targetPlayerId, CancellationToken cancellationToken = default)
    {
        var existingRequest = await _dbContext.ProfileMergeRequests
            .FirstOrDefaultAsync(r => r.UserId == userId && r.TargetPlayerId == targetPlayerId, cancellationToken);

        if (existingRequest != null)
        {
            existingRequest.Status = RegistrationStatus.Ignored;
        }
        else
        {
            var request = new ProfileMergeRequest
            {
                UserId = userId,
                TargetPlayerId = targetPlayerId,
                Status = RegistrationStatus.Ignored
            };
            _dbContext.ProfileMergeRequests.Add(request);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task MergePlayersDataAsync(long sourceId, long targetId, CancellationToken cancellationToken)
    {
        // Update all related tables - Reassigning everything from the "new" profile back to the "archival" one
        // Chess
        var chessPart = await _dbContext.ChessEventParticipants.Where(p => p.PlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var p in chessPart) p.PlayerId = targetId;
        var chessGamesW = await _dbContext.ChessGames.Where(g => g.WhitePlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var g in chessGamesW) g.WhitePlayerId = targetId;
        var chessGamesB = await _dbContext.ChessGames.Where(g => g.BlackPlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var g in chessGamesB) g.BlackPlayerId = targetId;
        // Checkers
        var checkersPart = await _dbContext.CheckersEventParticipants.Where(p => p.PlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var p in checkersPart) p.PlayerId = targetId;
        var checkersGamesW = await _dbContext.CheckersGames.Where(g => g.WhitePlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var g in checkersGamesW) g.WhitePlayerId = targetId;
        var checkersGamesB = await _dbContext.CheckersGames.Where(g => g.BlackPlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var g in checkersGamesB) g.BlackPlayerId = targetId;
        // Table Tennis
        var ttPart = await _dbContext.TableTennisEventParticipants.Where(p => p.PlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var p in ttPart) p.PlayerId = targetId;
        var ttMatches1 = await _dbContext.TableTennisMatches.Where(m => m.Player1Id == sourceId).ToListAsync(cancellationToken);
        foreach (var m in ttMatches1) m.Player1Id = targetId;
        var ttMatches2 = await _dbContext.TableTennisMatches.Where(m => m.Player2Id == sourceId).ToListAsync(cancellationToken);
        foreach (var m in ttMatches2) m.Player2Id = targetId;
        var ttWinner = await _dbContext.TableTennisMatches.Where(m => m.WinnerId == sourceId).ToListAsync(cancellationToken);
        foreach (var m in ttWinner) m.WinnerId = targetId;
        // Armwrestling
        var armPart = await _dbContext.ArmwrestlingEventParticipants.Where(p => p.PlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var p in armPart) p.PlayerId = targetId;
        var armMatches1 = await _dbContext.ArmwrestlingMatches.Where(m => m.Player1Id == sourceId).ToListAsync(cancellationToken);
        foreach (var m in armMatches1) m.Player1Id = targetId;
        var armMatches2 = await _dbContext.ArmwrestlingMatches.Where(m => m.Player2Id == sourceId).ToListAsync(cancellationToken);
        foreach (var m in armMatches2) m.Player2Id = targetId;
        var armWinner = await _dbContext.ArmwrestlingMatches.Where(m => m.WinnerId == sourceId).ToListAsync(cancellationToken);
        foreach (var m in armWinner) m.WinnerId = targetId;
        // Badminton
        var badPart = await _dbContext.BadmintonEventParticipants.Where(p => p.PlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var p in badPart) p.PlayerId = targetId;
        var badMatches1 = await _dbContext.BadmintonMatches.Where(m => m.Player1Id == sourceId).ToListAsync(cancellationToken);
        foreach (var m in badMatches1) m.Player1Id = targetId;
        var badMatches2 = await _dbContext.BadmintonMatches.Where(m => m.Player2Id == sourceId).ToListAsync(cancellationToken);
        foreach (var m in badMatches2) m.Player2Id = targetId;
        // Football Goals
        var goals = await _dbContext.FootballGoals.Where(g => g.PlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var g in goals) g.PlayerId = targetId;
        
        // Football Cards
        var cards = await _dbContext.FootballCards.Where(c => c.PlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var c in cards) c.PlayerId = targetId;

        // Football Event Participants
        var footballParts = await _dbContext.FootballEventParticipants.Where(p => p.PlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var p in footballParts) p.PlayerId = targetId;

        // Athletics Results
        var athleticsResults = await _dbContext.AthleticsResults.Where(r => r.PlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var r in athleticsResults) r.PlayerId = targetId;

        // Volleyball Event Participants
        var volleyParts = await _dbContext.VolleyballEventParticipants.Where(p => p.PlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var p in volleyParts) p.PlayerId = targetId;

        // Basketball Event Participants
        var bballParts = await _dbContext.Basketball3x3EventParticipants.Where(p => p.PlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var p in bballParts) p.PlayerId = targetId;

        // Beach Volleyball Event Participants
        var beachVParts = await _dbContext.BeachVolleyballEventParticipants.Where(p => p.PlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var p in beachVParts) p.PlayerId = targetId;

        // Badminton Event Participants
        var badParts = await _dbContext.BadmintonEventParticipants.Where(p => p.PlayerId == sourceId).ToListAsync(cancellationToken);
        foreach (var p in badParts) p.PlayerId = targetId;

        // General Event Winners
        var events1 = await _dbContext.Set<Event>().Where(e => e.FirstPlaceId == sourceId).ToListAsync(cancellationToken);
        foreach (var e in events1) e.FirstPlaceId = targetId;
        var events2 = await _dbContext.Set<Event>().Where(e => e.SecondPlaceId == sourceId).ToListAsync(cancellationToken);
        foreach (var e in events2) e.SecondPlaceId = targetId;
        var events3 = await _dbContext.Set<Event>().Where(e => e.ThirdPlaceId == sourceId).ToListAsync(cancellationToken);
        foreach (var e in events3) e.ThirdPlaceId = targetId;
    }

    private async Task PopulateAutomaticBadgesAsync(PlayerDto dto, CancellationToken cancellationToken)
    {
        if (dto.Id == null) return;
        var playerId = dto.Id.Value;
        var badges = new List<AutomaticBadgeDto>();
        try {
            // 1. All-time Top Scorer
            var allTimeTopScorer = await _dbContext.FootballGoals
                .GroupBy(g => g.PlayerId)
                .Select(group => new { PlayerId = group.Key, TotalGoals = group.Sum(g => g.GoalsCount) })
                .OrderByDescending(x => x.TotalGoals)
                .FirstOrDefaultAsync(cancellationToken);
            if (allTimeTopScorer != null && allTimeTopScorer.PlayerId == playerId && allTimeTopScorer.TotalGoals > 0)
            {
                badges.Add(new AutomaticBadgeDto {
                    Type = "TopScorer",
                    Label = "Найкращий бомбардир в історії",
                    Icon = "fire"
                });
            }
            // 2. Season Top Scorers
            var seasonTopScorersData = await _dbContext.FootballGoals
                .Include(g => g.Match)
                    .ThenInclude(m => m.Event)
                        .ThenInclude(e => e.Season)
                .Where(g => g.Match != null && g.Match.Event != null && g.Match.Event.SeasonId != null)
                .GroupBy(g => new { g.PlayerId, g.Match.Event.SeasonId, SeasonName = g.Match.Event.Season.Name })
                .Select(group => new {
                    PlayerId = group.Key.PlayerId,
                    SeasonId = group.Key.SeasonId,
                    SeasonName = group.Key.SeasonName,
                    TotalGoals = group.Sum(g => g.GoalsCount)
                })
                .ToListAsync(cancellationToken);
            var topPerSeason = seasonTopScorersData
                .GroupBy(s => s.SeasonId)
                .Select(g => g.OrderByDescending(x => x.TotalGoals).First())
                .Where(x => x.PlayerId == playerId && x.TotalGoals > 0);
            foreach (var top in topPerSeason)
            {
                badges.Add(new AutomaticBadgeDto {
                    Type = "SeasonTopScorer",
                    Label = $"Найкращий бомбардир ({top.SeasonName})",
                    Icon = "soccer-ball"
                });
            }

            // 3. Overall Season Winners
            var allSeasons = await _dbContext.Seasons
                .OrderByDescending(s => s.StartDate)
                .Take(5) // Limit to last 5 seasons for performance
                .ToListAsync(cancellationToken);

            foreach (var season in allSeasons)
            {
                try {
                    var rankings = await GetAllWithRankingsAsync(season.Id, cancellationToken);
                    var meInSeason = rankings.FirstOrDefault(p => p.Id == playerId);
                    if (meInSeason == null) continue;

                    if (meInSeason.GlobalRank == 1 && meInSeason.PerformanceScore > 0)
                    {
                        badges.Add(new AutomaticBadgeDto
                        {
                            Type = "BestAthlete",
                            Label = "Найкращий спортсмен",
                            SeasonName = season.Name,
                            Icon = "crown"
                        });
                    }

                    foreach (var sport in meInSeason.SportStats)
                    {
                        if (sport.Rank == 1 && sport.PerformanceScore > 0)
                        {
                            badges.Add(new AutomaticBadgeDto
                            {
                                Type = "BestAthleteSport",
                                Label = $"Найкращий гравець ({GetSportNameUa(sport.SportType)})",
                                SeasonName = season.Name,
                                Icon = "star"
                            });
                        }
                    }
                } catch (Exception ex) {
                    _logger.LogWarning(ex, "Error getting rankings for season {SeasonId} in badges population", season.Id);
                }
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error populating automatic badges for player {PlayerId}", playerId);
        }
        dto.AutomaticBadges = badges;
    }


    private string GetSportNameUa(string type)
    {
        return type.ToLower() switch
        {
            "chess" => "Шахи",
            "checkers" => "Шашки",
            "table_tennis" => "Настільний теніс",
            "armwrestling" => "Армрестлінг",
            "badminton" => "Бадмінтон",
            "football" => "Футбол",
            "volleyball" => "Волейбол",
            "basketball_3x3" => "Баскетбол 3x3",
            "beach_volleyball" => "Пляжний волейбол",
            "athletics" => "Легка атлетика",
            _ => type
        };
    }

    private async Task PopulateChessSpecialStatsAsync(PlayerDto dto, CancellationToken cancellationToken)
    {
        if (dto.Id == null) return;
        var playerId = dto.Id.Value;
        try
        {
            // Update ChessRating to be the highest rating from any chess tournament
            var maxRating = await _dbContext.ChessEventParticipants
                .Where(p => p.PlayerId == playerId)
                .MaxAsync(p => (int?)p.Rating, cancellationToken);
                
            if (maxRating.HasValue && maxRating > (dto.ChessRating ?? 0))
            {
                dto.ChessRating = maxRating.Value;
            }

            // 1. Get all inter-group chess performances for badges
            var chessPerformances = await _dbContext.ChessEventParticipants
                .Include(p => p.Tournament)
                .Where(p => p.PlayerId == playerId && p.Tournament != null && p.Tournament.Level == CompetitionLevel.CollegeGroup)
                .OrderByDescending(p => p.Tournament!.Date)
                .ToListAsync(cancellationToken);
            _logger.LogInformation("Found {Count} chess performances for player {PlayerId}", chessPerformances.Count, playerId);
            if (!chessPerformances.Any()) return;
            // 2. Check for 4 total 1st places
            var wins = chessPerformances.Where(p => p.Rank == 1 || (p.Tournament != null && p.Tournament.FirstPlaceId == playerId)).ToList();
            int totalWins = wins.Count;
            if (totalWins >= 4)
            {
                if (!dto.AutomaticBadges.Any(b => b.Type == "ChessTotalWins4"))
                {
                    dto.AutomaticBadges.Add(new AutomaticBadgeDto
                    {
                        Type = "ChessTotalWins4",
                        Label = "4-разовий переможець міжгрупових шахів",
                        Icon = "trophy"
                    });
                }
                if (!dto.Achievements.Any(a => a.TournamentName == "4-разовий переможець міжгрупових шахів"))
                {
                    dto.Achievements.Add(new AchievementDto
                    {
                        TournamentName = "4-разовий переможець міжгрупових шахів",
                        SportType = "chess",
                        Date = wins.OrderByDescending(p => p.Tournament!.Date).First().Tournament!.Date,
                        Rank = 1,
                        Result = "Спеціальне досягнення"
                    });
                }
            }
            // 3. Calculate total points and possible points
            double totalPoints = chessPerformances.Sum(p => p.Points);
            _logger.LogInformation("Player {PlayerId} has {Points} total points from {Count} performances", playerId, totalPoints, chessPerformances.Count);
            var tournamentIds = chessPerformances.Select(p => p.TournamentId).ToList();
            double totalPossible = chessPerformances.Count * 5;
            _logger.LogInformation("Player {PlayerId} total possible points calculated as {Possible}", playerId, totalPossible);
            if (totalPossible > 0 || totalPoints > 0)
            {
                dto.ChessTotalPoints = totalPoints;
                dto.ChessTotalPossiblePoints = totalPossible > 0 ? totalPossible : null;
            }
            // 4. Undefeated in 4 seasons
            // Re-fetch games for this player to check for losses
            var games = await _dbContext.ChessGames
                .Where(g => tournamentIds.Contains(g.TournamentId) && (g.WhitePlayerId == playerId || g.BlackPlayerId == playerId))
                .ToListAsync(cancellationToken);
            var seasonsWithChess = await _dbContext.Seasons
                .Where(s => _dbContext.ChessEvents.Any(e => e.SeasonId == s.Id && e.Level == CompetitionLevel.CollegeGroup &&
                    _dbContext.ChessEventParticipants.Any(p => p.TournamentId == e.Id && p.PlayerId == playerId)))
                .ToListAsync(cancellationToken);
            int undefeatedSeasons = 0;
            foreach (var season in seasonsWithChess)
            {
                var seasonEventIds = await _dbContext.ChessEvents
                    .Where(e => e.SeasonId == season.Id && e.Level == CompetitionLevel.CollegeGroup)
                    .Select(e => e.Id)
                    .ToListAsync(cancellationToken);
                var seasonGames = games.Where(g => seasonEventIds.Contains(g.TournamentId)).ToList();
                if (!seasonGames.Any()) continue;
                bool lost = seasonGames.Any(g =>
                    (g.WhitePlayerId == playerId && g.Result == GameResult.BlackWin) ||
                    (g.BlackPlayerId == playerId && g.Result == GameResult.WhiteWin)
                );
                if (!lost) undefeatedSeasons++;
            }
            if (undefeatedSeasons >= 4)
            {
                dto.IsChessUndefeatedFourSeasons = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error populating special chess stats for player {PlayerId}", playerId);
        }
    }

    public async Task<HeadToHeadDto> GetHeadToHeadAsync(long player1Id, long player2Id, CancellationToken cancellationToken)
    {
        var result = new HeadToHeadDto
        {
            Player1Id = player1Id,
            Player2Id = player2Id
        };

        // 1. Chess
        var chessMatches = await _dbContext.ChessGames
            .Include(g => g.Tournament)
            .Where(g => (g.WhitePlayerId == player1Id && g.BlackPlayerId == player2Id) ||
                        (g.WhitePlayerId == player2Id && g.BlackPlayerId == player1Id))
            .ToListAsync(cancellationToken);

        foreach (var m in chessMatches)
        {
            var matchDto = new HeadToHeadMatchDto {
                SportType = "chess",
                TournamentName = m.Tournament?.Name ?? "Турнір з шахів",
                Date = m.Tournament?.Date ?? DateTime.MinValue,
                TournamentId = m.TournamentId,
                Score = m.Result.ToString()
            };
            bool p1IsWhite = m.WhitePlayerId == player1Id;
            if (m.Result == GameResult.Draw) { result.Draws++; matchDto.Result = "Draw"; }
            else if ((p1IsWhite && m.Result == GameResult.WhiteWin) || (!p1IsWhite && m.Result == GameResult.BlackWin)) { result.Player1Wins++; matchDto.Result = "P1 Won"; }
            else { result.Player2Wins++; matchDto.Result = "P2 Won"; }
            result.Matches.Add(matchDto);
        }

        // 2. Checkers
        var checkersMatches = await _dbContext.CheckersGames
            .Include(g => g.Event)
            .Include(g => g.WhitePlayer)
            .Include(g => g.BlackPlayer)
            .Where(g => (g.WhitePlayer.PlayerId == player1Id && g.BlackPlayer.PlayerId == player2Id) ||
                        (g.WhitePlayer.PlayerId == player2Id && g.BlackPlayer.PlayerId == player1Id))
            .ToListAsync(cancellationToken);

        foreach (var m in checkersMatches)
        {
            var matchDto = new HeadToHeadMatchDto {
                SportType = "checkers",
                TournamentName = m.Event?.Name ?? "Турнір з Шашок",
                Date = m.Event?.Date ?? DateTime.MinValue,
                TournamentId = m.TournamentId,
                Score = m.Result.ToString()
            };
            bool p1IsWhite = m.WhitePlayer.PlayerId == player1Id;
            if (m.Result == GameResult.Draw) { result.Draws++; matchDto.Result = "Draw"; }
            else if ((p1IsWhite && m.Result == GameResult.WhiteWin) || (!p1IsWhite && m.Result == GameResult.BlackWin)) { result.Player1Wins++; matchDto.Result = "P1 Won"; }
            else { result.Player2Wins++; matchDto.Result = "P2 Won"; }
            result.Matches.Add(matchDto);
        }

        // 3. Table Tennis
        var ttMatches = await _dbContext.TableTennisMatches
            .Include(m => m.Tournament)
            .Where(m => (m.Player1Id == player1Id && m.Player2Id == player2Id) ||
                        (m.Player1Id == player2Id && m.Player2Id == player1Id))
            .ToListAsync(cancellationToken);
        
        foreach (var m in ttMatches)
        {
            var matchDto = new HeadToHeadMatchDto {
                SportType = "table_tennis",
                TournamentName = m.Tournament?.Name ?? "Турнір з настільного тенісу",
                Date = m.Tournament?.Date ?? DateTime.MinValue,
                TournamentId = m.TournamentId,
                Score = $"{m.HomeTeamScore}:{m.AwayTeamScore}"
            };
            bool p1IsFirst = m.Player1Id == player1Id;
            if (m.WinnerId == player1Id) { result.Player1Wins++; matchDto.Result = "P1 Won"; }
            else if (m.WinnerId == player2Id) { result.Player2Wins++; matchDto.Result = "P2 Won"; }
            else { result.Draws++; matchDto.Result = "Draw"; }
            result.Matches.Add(matchDto);
        }

        // 4. Armwrestling
        var armMatches = await _dbContext.ArmwrestlingMatches
            .Include(m => m.Event)
            .Where(m => (m.Player1Id == player1Id && m.Player2Id == player2Id) ||
                        (m.Player1Id == player2Id && m.Player2Id == player1Id))
            .ToListAsync(cancellationToken);
            
        foreach (var m in armMatches)
        {
            var matchDto = new HeadToHeadMatchDto {
                SportType = "armwrestling",
                TournamentName = m.Event?.Name ?? "Турнір з Армрестлінгу",
                Date = m.Event?.Date ?? DateTime.MinValue,
                TournamentId = m.EventId,
                Score = m.Stage ?? "Match"
            };
            if (m.WinnerId == player1Id) { result.Player1Wins++; matchDto.Result = "P1 Won"; }
            else if (m.WinnerId == player2Id) { result.Player2Wins++; matchDto.Result = "P2 Won"; }
            else { result.Draws++; matchDto.Result = "Draw"; }
            result.Matches.Add(matchDto);
        }

        // 5. Team Sports (Football, Basketball, Volleyball)
        // Football
        var FootballMatches = await _dbContext.FootballMatches
            .Include(m => m.Event)
            .Include(m => m.HomeTeam).ThenInclude(t => t.Players)
            .Include(m => m.AwayTeam).ThenInclude(t => t.Players)
            .Include(m => m.HomeRoster)
            .Include(m => m.AwayRoster)
            .Where(m => (m.HomeTeam!.Players!.Any(p => p.Id == player1Id) && m.AwayTeam!.Players!.Any(p => p.Id == player2Id)) ||
                        (m.HomeTeam!.Players!.Any(p => p.Id == player2Id) && m.AwayTeam!.Players!.Any(p => p.Id == player1Id)) ||
                        (m.HomeRoster!.Any(p => p.Id == player1Id) && m.AwayRoster!.Any(p => p.Id == player2Id)) ||
                        (m.HomeRoster!.Any(p => p.Id == player2Id) && m.AwayRoster!.Any(p => p.Id == player1Id)))
            .ToListAsync(cancellationToken);

        foreach (var m in FootballMatches)
        {
            var matchDto = new HeadToHeadMatchDto {
                SportType = "football",
                TournamentName = m.Event?.Name ?? "Турнір з футболу",
                Date = m.Event?.Date ?? DateTime.MinValue,
                TournamentId = m.EventId,
                Score = $"{m.HomeTeamScore}:{m.AwayTeamScore}"
            };
            bool p1IsHome = (m.HomeTeam?.Players?.Any(p => p.Id == player1Id) ?? false) || (m.HomeRoster?.Any(p => p.Id == player1Id) ?? false);
            if (m.WinnerTeamId != null) {
                bool p1Won = (p1IsHome && m.WinnerTeamId == m.HomeTeamId) || (!p1IsHome && m.WinnerTeamId == m.AwayTeamId);
                if (p1Won) { result.Player1Wins++; matchDto.Result = "P1 Won"; } else { result.Player2Wins++; matchDto.Result = "P2 Won"; }
            } else { result.Draws++; matchDto.Result = "Draw"; }
            result.Matches.Add(matchDto);
        }

        // Basketball 3x3
        var basketballMatches = await _dbContext.Basketball3x3Matches
            .Include(m => m.Event)
            .Include(m => m.HomeTeam).ThenInclude(t => t.Players)
            .Include(m => m.AwayTeam).ThenInclude(t => t.Players)
            .Include(m => m.HomeRoster)
            .Include(m => m.AwayRoster)
            .Where(m => (m.HomeTeam!.Players!.Any(p => p.Id == player1Id) && m.AwayTeam!.Players!.Any(p => p.Id == player2Id)) ||
                        (m.HomeTeam!.Players!.Any(p => p.Id == player2Id) && m.AwayTeam!.Players!.Any(p => p.Id == player1Id)) ||
                        (m.HomeRoster!.Any(p => p.Id == player1Id) && m.AwayRoster!.Any(p => p.Id == player2Id)) ||
                        (m.HomeRoster!.Any(p => p.Id == player2Id) && m.AwayRoster!.Any(p => p.Id == player1Id)))
            .ToListAsync(cancellationToken);

        foreach (var m in basketballMatches)
        {
            var matchDto = new HeadToHeadMatchDto {
                SportType = "basketball_3x3",
                TournamentName = m.Event?.Name ?? "Турнір з Баскетболу 3х3",
                Date = m.Event?.Date ?? DateTime.MinValue,
                TournamentId = m.EventId,
                Score = $"{m.HomeTeamScore}:{m.AwayTeamScore}"
            };
            bool p1IsHome = (m.HomeTeam?.Players?.Any(p => p.Id == player1Id) ?? false) || (m.HomeRoster?.Any(p => p.Id == player1Id) ?? false);
            if (m.WinnerTeamId != null) {
                bool p1Won = (p1IsHome && m.WinnerTeamId == m.HomeTeamId) || (!p1IsHome && m.WinnerTeamId == m.AwayTeamId);
                if (p1Won) { result.Player1Wins++; matchDto.Result = "P1 Won"; } else { result.Player2Wins++; matchDto.Result = "P2 Won"; }
            } else { result.Draws++; matchDto.Result = "Draw"; }
            result.Matches.Add(matchDto);
        }

        // Volleyball
        var volleyballMatches = await _dbContext.VolleyballMatches
            .Include(m => m.Event)
            .Include(m => m.HomeTeam).ThenInclude(t => t.Players)
            .Include(m => m.AwayTeam).ThenInclude(t => t.Players)
            .Where(m => (m.HomeTeam!.Players!.Any(p => p.Id == player1Id) && m.AwayTeam!.Players!.Any(p => p.Id == player2Id)) ||
                        (m.HomeTeam!.Players!.Any(p => p.Id == player2Id) && m.AwayTeam!.Players!.Any(p => p.Id == player1Id)))
            .ToListAsync(cancellationToken);

        foreach (var m in volleyballMatches)
        {
            var matchDto = new HeadToHeadMatchDto {
                SportType = "volleyball",
                TournamentName = m.Event?.Name ?? "Турнір з волейболу",
                Date = m.Event?.Date ?? DateTime.MinValue,
                TournamentId = m.EventId,
                Score = $"{m.HomeSetsWon}:{m.AwaySetsWon}"
            };
            bool p1IsHome = (m.HomeTeam?.Players?.Any(p => p.Id == player1Id) ?? false) || (m.HomeRoster?.Any(p => p.Id == player1Id) ?? false);
            if (m.WinnerTeamId != null) {
                bool p1Won = (p1IsHome && m.WinnerTeamId == m.HomeTeamId) || (!p1IsHome && m.WinnerTeamId == m.AwayTeamId);
                if (p1Won) { result.Player1Wins++; matchDto.Result = "P1 Won"; } else { result.Player2Wins++; matchDto.Result = "P2 Won"; }
            } else { result.Draws++; matchDto.Result = "Draw"; }
            result.Matches.Add(matchDto);
        }

        // Beach Volleyball
        var beachVolleyballMatches = await _dbContext.BeachVolleyballMatches
            .Include(m => m.Event)
            .Include(m => m.HomeTeam).ThenInclude(t => t.Players)
            .Include(m => m.AwayTeam).ThenInclude(t => t.Players)
            .Include(m => m.HomeRoster)
            .Include(m => m.AwayRoster)
            .Where(m => (m.HomeTeam!.Players!.Any(p => p.Id == player1Id) && m.AwayTeam!.Players!.Any(p => p.Id == player2Id)) ||
                        (m.HomeTeam!.Players!.Any(p => p.Id == player2Id) && m.AwayTeam!.Players!.Any(p => p.Id == player1Id)) ||
                        (m.HomeRoster!.Any(p => p.Id == player1Id) && m.AwayRoster!.Any(p => p.Id == player2Id)) ||
                        (m.HomeRoster!.Any(p => p.Id == player2Id) && m.AwayRoster!.Any(p => p.Id == player1Id)))
            .ToListAsync(cancellationToken);

        foreach (var m in beachVolleyballMatches)
        {
            var matchDto = new HeadToHeadMatchDto {
                SportType = "beach_volleyball",
                TournamentName = m.Event?.Name ?? "Турнір з пляжного волейболу",
                Date = m.Event?.Date ?? DateTime.MinValue,
                TournamentId = m.EventId,
                Score = $"{m.HomeSetsWon}:{m.AwaySetsWon}"
            };
            bool p1IsHome = (m.HomeTeam?.Players?.Any(p => p.Id == player1Id) ?? false) || (m.HomeRoster?.Any(p => p.Id == player1Id) ?? false);
            if (m.WinnerTeamId != null) {
                bool p1Won = (p1IsHome && m.WinnerTeamId == m.HomeTeamId) || (!p1IsHome && m.WinnerTeamId == m.AwayTeamId);
                if (p1Won) { result.Player1Wins++; matchDto.Result = "P1 Won"; } else { result.Player2Wins++; matchDto.Result = "P2 Won"; }
            } else { result.Draws++; matchDto.Result = "Draw"; }
            result.Matches.Add(matchDto);
        }

        result.Matches = result.Matches.OrderByDescending(m => m.Date).ToList();
        return result;
    }

    private async Task PopulateFootballStatsAsync(PlayerDto dto, CancellationToken cancellationToken)
    {
        if (dto.Id == null)
        {
            _logger.LogWarning("PopulateFootballStatsAsync: Player ID is null for user {UserId}. Skipping stats population.", dto.UserId);
            return;
        }

        try
        {
            var playerId = dto.Id.Value;
            _logger.LogInformation("[DEBUG] PopulateFootballStatsAsync for Player ID: {PlayerId}, User ID: {UserId}", playerId, dto.UserId);
            
            var goals = await _dbContext.FootballGoals
                .Where(g => g.PlayerId == playerId)
                .SumAsync(g => (int?)g.GoalsCount, cancellationToken) ?? 0;
            
            dto.GoalsCount = goals;

            var goalDetails = await _dbContext.FootballGoals
                .Include(g => g.Match)
                    .ThenInclude(m => m!.Event)
                .Include(g => g.Match)
                    .ThenInclude(m => m!.HomeTeam)
                .Include(g => g.Match)
                    .ThenInclude(m => m!.AwayTeam)
                .Where(g => g.PlayerId == playerId)
                .ToListAsync(cancellationToken);

            dto.FootballGoalMatches = goalDetails.Select(g => new FootballGoalMatchDto
            {
                MatchId = g.MatchId,
                TournamentId = g.Match?.EventId ?? 0,
                TournamentName = g.Match?.Event?.Name ?? "Турнір з футболу",
                HomeTeamName = g.Match?.HomeTeam?.Name ?? "Домашня команда",
                AwayTeamName = g.Match?.AwayTeam?.Name ?? "Гостьова команда",
                GoalsScored = g.GoalsCount,
                Date = g.Match?.Event?.Date ?? DateTime.MinValue
            }).OrderByDescending(x => x.Date).ToList();
            
            // Try fetching cards using multiple methods to be absolutely sure
            int yellowCards = 0;
            int redCards = 0;
            
            try 
            {
                // Method 1: LINQ with explicit int conversion
                var playerCards = await _dbContext.FootballCards
                    .AsNoTracking()
                    .Where(c => c.PlayerId == playerId)
                    .Select(c => new { c.Type })
                    .ToListAsync(cancellationToken);

                yellowCards = playerCards.Count(c => (int)c.Type == 1);
                redCards = playerCards.Count(c => (int)c.Type == 2);
                
                _logger.LogInformation("[DEBUG] Stats for Player {Id}: LINQ found {Count} cards. Y: {Yellow}, R: {Red}", 
                    playerId, playerCards.Count, yellowCards, redCards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LINQ card counting failed for player {Id}", playerId);
            }

            // DIAGNOSTIC: Total cards in table
            try {
                var globalCount = await _dbContext.FootballCards.CountAsync(cancellationToken);
                _logger.LogInformation("[DEBUG] GLOBAL FootballCards count in DB: {Count}", globalCount);
            } catch { }

            dto.YellowCardsCount = yellowCards;
            dto.RedCardsCount = redCards;

            // Always add football to sportStats if they have any stats or team info
            bool isFootballer = goals > 0 || yellowCards > 0 || redCards > 0 || 
                                dto.FootballTeamId != null || dto.FootballPosition != null || 
                                dto.FootballShirtNumber != null;

            if (isFootballer)
            {
                var footballStats = dto.SportStats?.FirstOrDefault(s => s.SportType.Equals("football", StringComparison.OrdinalIgnoreCase));
                if (footballStats == null)
                {
                    footballStats = new SportStatsDto { SportType = "football" };
                    dto.SportStats ??= new List<SportStatsDto>();
                    dto.SportStats.Add(footballStats);
                }
                footballStats.GoalsCount = goals;
                footballStats.YellowCardsCount = yellowCards;
                footballStats.RedCardsCount = redCards;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error populating football stats for player {PlayerId}", dto.Id);
        }
    }
}
