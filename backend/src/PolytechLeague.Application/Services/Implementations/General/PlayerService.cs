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
using PolytechLeague.Domain.Entities.Futsal;
using PolytechLeague.Domain.Entities.Volleyball;
using PolytechLeague.Domain.Entities.Basketball3x3;
using PolytechLeague.Domain.Entities.BeachVolleyball;
using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Application.Services.Implementations.General;

public class PlayerService : IPlayerService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(ApplicationDbContext dbContext, ILogger<PlayerService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<PlayerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var players = await _dbContext.Players
            .ToListAsync(cancellationToken);
        return players.Select(p => p.ToDto());
    }

    public async Task<PlayerDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var player = await _dbContext.Players
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (player == null) return null;
        
        var dto = player.ToDto();
        try {
            await PopulateAchievementsAsync(dto, cancellationToken);
            await PopulateStatsAsync(dto, cancellationToken);
        } catch (Exception ex) {
            // Log error but allow profile to load with empty stats
            Console.WriteLine($"Error populating profile metrics: {ex.Message}");
        }
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
        var player = await _dbContext.Players
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        if (player == null) return null;
        
        var dto = player.ToDto();
        try {
            await PopulateAchievementsAsync(dto, cancellationToken);
            await PopulateStatsAsync(dto, cancellationToken);
        } catch (Exception ex) {
            // Log error but allow profile to load with empty stats
            Console.WriteLine($"Error populating profile metrics: {ex.Message}");
        }
        return dto;
    }

    public async Task<PlayerDto> CreateAsync(PlayerDto dto, CancellationToken cancellationToken = default)
    {
        var player = dto.FromDto();
        await _dbContext.Players.AddAsync(player, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return player.ToDto();
    }

    public async Task UpdateAsync(long id, PlayerDto dto, CancellationToken cancellationToken = default)
    {
        var player = await _dbContext.Players.FindAsync(new object[] { id }, cancellationToken);
        if (player == null) return;

        // Update basic info
        player.FirstName = dto.FirstName ?? player.FirstName;
        player.Patronymic = dto.Patronymic ?? player.Patronymic;
        player.LastName = dto.LastName ?? player.LastName;
        player.Group = dto.Group ?? player.Group;
        player.StudentId = dto.StudentId ?? player.StudentId;
        player.City = dto.City ?? player.City;
        player.UserId = dto.UserId ?? player.UserId;
        player.Faculty = dto.Faculty ?? player.Faculty;
        player.Bio = dto.Bio ?? player.Bio;
        player.InstagramUrl = dto.InstagramUrl ?? player.InstagramUrl;
        player.PhotoUrl = dto.PhotoUrl ?? player.PhotoUrl;

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
        if (dto.FutsalPosition.HasValue) player.FutsalPosition = dto.FutsalPosition;
        if (dto.FutsalShirtNumber.HasValue) player.FutsalShirtNumber = dto.FutsalShirtNumber;
        if (dto.FutsalTeamId.HasValue) player.FutsalTeamId = dto.FutsalTeamId;
        
        if (dto.VolleyballPosition.HasValue) player.VolleyballPosition = dto.VolleyballPosition;
        if (dto.VolleyballShirtNumber.HasValue) player.VolleyballShirtNumber = dto.VolleyballShirtNumber;
        if (dto.VolleyballTeamId.HasValue) player.VolleyballTeamId = dto.VolleyballTeamId;

        if (dto.BasketballShirtNumber.HasValue) player.BasketballShirtNumber = dto.BasketballShirtNumber;
        if (dto.BasketballTeamId.HasValue) player.BasketballTeamId = dto.BasketballTeamId;

        if (dto.BeachVolleyballShirtNumber.HasValue) player.BeachVolleyballShirtNumber = dto.BeachVolleyballShirtNumber;
        if (dto.BeachVolleyballTeamId.HasValue) player.BeachVolleyballTeamId = dto.BeachVolleyballTeamId;

        await _dbContext.SaveChangesAsync(cancellationToken);
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
            .Where(p => p.FirstName.ToLower().Contains(lowerQuery) || 
                        (p.Patronymic != null && p.Patronymic.ToLower().Contains(lowerQuery)) ||
                        p.LastName.ToLower().Contains(lowerQuery) ||
                        p.Group.ToLower().Contains(lowerQuery) ||
                        (p.StudentId != null && p.StudentId.ToLower().Contains(lowerQuery)))
            .ToListAsync(cancellationToken);
        
        return players.Select(p => p.ToDto());
    }

    public async Task<IEnumerable<PlayerDto>> GetAllWithRankingsAsync(CancellationToken cancellationToken = default)
    {
        try 
        {
            var players = await _dbContext.Players.Include(p => p.User).ToListAsync(cancellationToken);
            if (!players.Any()) return Enumerable.Empty<PlayerDto>();

            var playerIds = players.Select(p => p.Id).ToList();

            // 1. Level multipliers
            var levelMultipliers = new Dictionary<CompetitionLevel, double> {
                { CompetitionLevel.CollegeGroup, 1.0 },
                { CompetitionLevel.Regional, 2.5 },
                { CompetitionLevel.National, 5.0 }
            };

            var allEvents = await _dbContext.Set<Event>()
                .Select(e => new { e.Id, e.Level })
                .ToDictionaryAsync(e => e.Id, e => e.Level, cancellationToken);

            // 2. Achievements
            var chessAch = await _dbContext.ChessEventParticipants.Include(p => p.Tournament).Where(p => playerIds.Contains(p.PlayerId)).ToListAsync(cancellationToken);
            var checkersAch = await _dbContext.CheckersEventParticipants.Include(p => p.Event).Where(p => playerIds.Contains(p.PlayerId)).ToListAsync(cancellationToken);
            var ttAch = await _dbContext.TableTennisEventParticipants.Include(p => p.Tournament).Where(p => playerIds.Contains(p.PlayerId)).ToListAsync(cancellationToken);
            var badmintonAch = await _dbContext.BadmintonEventParticipants.Include(p => p.Event).Where(p => playerIds.Contains(p.PlayerId)).ToListAsync(cancellationToken);
            
            var genericWinners = await _dbContext.Set<Event>()
                .Where(e => (e.FirstPlaceId.HasValue && playerIds.Contains(e.FirstPlaceId.Value)) || 
                            (e.SecondPlaceId.HasValue && playerIds.Contains(e.SecondPlaceId.Value)) || 
                            (e.ThirdPlaceId.HasValue && playerIds.Contains(e.ThirdPlaceId.Value)))
                .ToListAsync(cancellationToken);

            var futsalTeams = await _dbContext.FutsalTeams.Include(t => t.Event).Include(t => t.Players).Where(t => t.Players.Any(p => playerIds.Contains(p.Id))).ToListAsync(cancellationToken);
            var volleyballTeams = await _dbContext.VolleyballTeams.Include(t => t.Event).Include(t => t.Players).Where(t => t.Players.Any(p => playerIds.Contains(p.Id))).ToListAsync(cancellationToken);
            var bballTeams = await _dbContext.Basketball3x3Teams.Include(t => t.Event).Include(t => t.Players).Where(t => t.Players.Any(p => playerIds.Contains(p.Id))).ToListAsync(cancellationToken);
            var beachVTeams = await _dbContext.BeachVolleyballTeams.Include(t => t.Event).Include(t => t.Players).Where(t => t.Players.Any(p => playerIds.Contains(p.Id))).ToListAsync(cancellationToken);

            // 3. Match Stats
            var cwS = await _dbContext.ChessGames.Where(g => g.WhitePlayerId.HasValue && playerIds.Contains(g.WhitePlayerId.Value)).GroupBy(g => g.WhitePlayerId!.Value).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.Result == GameResult.WhiteWin) }).ToListAsync(cancellationToken);
            var cbS = await _dbContext.ChessGames.Where(g => g.BlackPlayerId.HasValue && playerIds.Contains(g.BlackPlayerId.Value)).GroupBy(g => g.BlackPlayerId!.Value).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.Result == GameResult.BlackWin) }).ToListAsync(cancellationToken);
            var chwS = await _dbContext.CheckersGames.Where(g => g.WhitePlayerId.HasValue && playerIds.Contains(g.WhitePlayerId.Value)).GroupBy(g => g.WhitePlayerId!.Value).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.Result == GameResult.WhiteWin) }).ToListAsync(cancellationToken);
            var chbS = await _dbContext.CheckersGames.Where(g => g.BlackPlayerId.HasValue && playerIds.Contains(g.BlackPlayerId.Value)).GroupBy(g => g.BlackPlayerId!.Value).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.Result == GameResult.BlackWin) }).ToListAsync(cancellationToken);
            var tt1S = await _dbContext.TableTennisMatches.Where(g => playerIds.Contains(g.Player1Id)).GroupBy(g => g.Player1Id).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.WinnerId == g.Key) }).ToListAsync(cancellationToken);
            var tt2S = await _dbContext.TableTennisMatches.Where(g => playerIds.Contains(g.Player2Id)).GroupBy(g => g.Player2Id).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.WinnerId == g.Key) }).ToListAsync(cancellationToken);
            var arm1S = await _dbContext.ArmwrestlingMatches.Where(g => playerIds.Contains(g.Player1Id)).GroupBy(g => g.Player1Id).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.WinnerId == g.Key) }).ToListAsync(cancellationToken);
            var arm2S = await _dbContext.ArmwrestlingMatches.Where(g => playerIds.Contains(g.Player2Id)).GroupBy(g => g.Player2Id).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.WinnerId == g.Key) }).ToListAsync(cancellationToken);
            var bad1S = await _dbContext.BadmintonMatches.Where(g => g.Player1Id.HasValue && playerIds.Contains(g.Player1Id.Value)).GroupBy(g => g.Player1Id!.Value).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.WinnerPlayerId == g.Key) }).ToListAsync(cancellationToken);
            var bad2S = await _dbContext.BadmintonMatches.Where(g => g.Player2Id.HasValue && playerIds.Contains(g.Player2Id.Value)).GroupBy(g => g.Player2Id!.Value).Select(g => new { Id = g.Key, Count = g.Count(), Wins = g.Count(x => x.WinnerPlayerId == g.Key) }).ToListAsync(cancellationToken);
            
            var goalsS = await _dbContext.FutsalGoals
                .Where(g => playerIds.Contains(g.PlayerId))
                .GroupBy(g => g.PlayerId)
                .Select(g => new { Id = g.Key, Goals = g.Sum(x => x.GoalsCount) })
                .ToListAsync(cancellationToken);

            var playerDtos = new List<PlayerDto>();

            foreach (var player in players)
            {
                var dto = player.ToDto();
                var playerId = player.Id;
                var achievements = new List<AchievementDto>();
                var sportTypes = new HashSet<string>();

                foreach (var a in chessAch.Where(x => x.PlayerId == playerId && x.Tournament != null)) {
                    achievements.Add(new AchievementDto { TournamentName = a.Tournament!.Name, SportType = "chess", Date = a.Tournament.Date, Rank = a.Rank, TournamentId = a.TournamentId });
                    sportTypes.Add("chess");
                }
                foreach (var a in checkersAch.Where(x => x.PlayerId == playerId && x.Event != null)) {
                    achievements.Add(new AchievementDto { TournamentName = a.Event!.Name, SportType = "checkers", Date = a.Event.Date, Rank = a.Rank, TournamentId = a.TournamentId });
                    sportTypes.Add("checkers");
                }
                foreach (var a in ttAch.Where(x => x.PlayerId == playerId && x.Tournament != null)) {
                    achievements.Add(new AchievementDto { TournamentName = a.Tournament!.Name, SportType = "table_tennis", Date = a.Tournament.Date, Rank = a.Rank, TournamentId = a.TournamentId });
                    sportTypes.Add("table_tennis");
                }
                foreach (var a in badmintonAch.Where(x => x.PlayerId == playerId && x.Event != null)) {
                    achievements.Add(new AchievementDto { TournamentName = a.Event!.Name, SportType = "badminton", Date = a.Event.Date, Rank = a.Rank, TournamentId = a.EventId });
                    sportTypes.Add("badminton");
                }
                foreach (var e in genericWinners.Where(x => x.FirstPlaceId == playerId || x.SecondPlaceId == playerId || x.ThirdPlaceId == playerId)) {
                    int r = (e.FirstPlaceId == playerId) ? 1 : ((e.SecondPlaceId == playerId) ? 2 : 3);
                    achievements.Add(new AchievementDto { TournamentName = e.Name, SportType = "armwrestling", Date = e.Date, Rank = r, TournamentId = e.Id });
                    sportTypes.Add("armwrestling");
                }

                // Team Sports
                foreach (var t in futsalTeams.Where(x => x.Players.Any(p => p.Id == playerId) && x.Event != null)) {
                    int r = (t.Id == t.Event!.FirstPlaceTeamId) ? 1 : ((t.Id == t.Event.SecondPlaceTeamId) ? 2 : ((t.Id == t.Event.ThirdPlaceTeamId) ? 3 : 0));
                    achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "futsal", Date = t.Event.Date, Rank = r, TournamentId = t.EventId });
                    sportTypes.Add("futsal");
                }
                foreach (var t in volleyballTeams.Where(x => x.Players.Any(p => p.Id == playerId) && x.Event != null)) {
                    int r = (t.Id == t.Event!.FirstPlaceTeamId) ? 1 : ((t.Id == t.Event.SecondPlaceTeamId) ? 2 : ((t.Id == t.Event.ThirdPlaceTeamId) ? 3 : 0));
                    achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "volleyball", Date = t.Event.Date, Rank = r, TournamentId = t.EventId });
                    sportTypes.Add("volleyball");
                }
                foreach (var t in bballTeams.Where(x => x.Players.Any(p => p.Id == playerId) && x.Event != null)) {
                    int r = (t.Id == t.Event!.FirstPlaceTeamId) ? 1 : ((t.Id == t.Event.SecondPlaceTeamId) ? 2 : ((t.Id == t.Event.ThirdPlaceTeamId) ? 3 : 0));
                    achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "basketball_3x3", Date = t.Event.Date, Rank = r, TournamentId = t.EventId });
                    sportTypes.Add("basketball_3x3");
                }
                foreach (var t in beachVTeams.Where(x => x.Players.Any(p => p.Id == playerId) && x.Event != null)) {
                    int r = (t.Id == t.Event!.FirstPlaceTeamId) ? 1 : ((t.Id == t.Event.SecondPlaceTeamId) ? 2 : ((t.Id == t.Event.ThirdPlaceTeamId) ? 3 : 0));
                    achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "beach_volleyball", Date = t.Event.Date, Rank = r, TournamentId = t.EventId });
                    sportTypes.Add("beach_volleyball");
                }

                dto.Achievements = achievements;
                dto.Sports = sportTypes.ToList();

                // Stats
                int played = 0, won = 0;
                double totalScore = 0;
                var sportStatsMap = new Dictionary<string, SportStatsDto>();
                var pointsBreakdown = new List<PointEntryDto>();
                foreach (var sport in sportTypes) sportStatsMap[sport] = new SportStatsDto { SportType = sport };

                foreach (var ach in achievements) {
                    double pts = (ach.Rank == 1) ? 100 : (ach.Rank == 2 ? 50 : (ach.Rank == 3 ? 30 : 0));
                    if (pts <= 0) continue;
                    
                    double mult = 1.0;
                    string levelName = "Міжгрупові";
                    if (allEvents.TryGetValue(ach.TournamentId, out var lvl)) {
                        if (levelMultipliers.TryGetValue(lvl, out var m)) mult = m;
                        levelName = lvl switch {
                            CompetitionLevel.Regional => "Обласні",
                            CompetitionLevel.National => "Всеукраїнські",
                            _ => "Міжгрупові"
                        };
                    }
                    var finalPts = (int)Math.Round(pts * mult);
                    totalScore += finalPts;
                    if (sportStatsMap.TryGetValue(ach.SportType, out var s)) s.PerformanceScore += finalPts;
                    
                    pointsBreakdown.Add(new PointEntryDto {
                        Description = $"{ach.Rank} місце: {ach.TournamentName} ({levelName})",
                        Points = finalPts,
                        Category = "Achievements"
                    });
                }

                void AddStats(string sport, int p, int w) {
                    played += p; won += w;
                    if (p > 0) {
                        if (!sportStatsMap.ContainsKey(sport)) sportStatsMap[sport] = new SportStatsDto { SportType = sport };
                        var s = sportStatsMap[sport];
                        s.MatchesPlayed += p; s.WinsCount += w;
                        
                        int matchPts = p * 1;
                        int winPts = w * 2;
                        int totalPts = matchPts + winPts;
                        
                        s.PerformanceScore += totalPts; 
                        totalScore += totalPts;
                        
                        string sportName = sport switch {
                            "chess" => "Шахи", "checkers" => "Шашки", "table_tennis" => "Настільний теніс",
                            "armwrestling" => "Армрестлинг", "badminton" => "Бадмінтон", _ => sport
                        };
                        
                        if (matchPts > 0) pointsBreakdown.Add(new PointEntryDto { Description = $"Участь у матчах ({sportName}): {p} ігор", Points = matchPts, Category = "Activity" });
                        if (winPts > 0) pointsBreakdown.Add(new PointEntryDto { Description = $"Перемоги ({sportName}): {w} ігор", Points = winPts, Category = "Activity" });
                    }
                }

                var cw = cwS.FirstOrDefault(x => x.Id == playerId);
                var cb = cbS.FirstOrDefault(x => x.Id == playerId);
                var chw = chwS.FirstOrDefault(x => x.Id == playerId);
                var chb = chbS.FirstOrDefault(x => x.Id == playerId);
                var t1 = tt1S.FirstOrDefault(x => x.Id == playerId);
                var t2 = tt2S.FirstOrDefault(x => x.Id == playerId);
                var a1 = arm1S.FirstOrDefault(x => x.Id == playerId);
                var a2 = arm2S.FirstOrDefault(x => x.Id == playerId);
                var b1 = bad1S.FirstOrDefault(x => x.Id == playerId);
                var b2 = bad2S.FirstOrDefault(x => x.Id == playerId);

                AddStats("chess", (cw?.Count ?? 0) + (cb?.Count ?? 0), (cw?.Wins ?? 0) + (cb?.Wins ?? 0));
                AddStats("checkers", (chw?.Count ?? 0) + (chb?.Count ?? 0), (chw?.Wins ?? 0) + (chb?.Wins ?? 0));
                AddStats("table_tennis", (t1?.Count ?? 0) + (t2?.Count ?? 0), (t1?.Wins ?? 0) + (t2?.Wins ?? 0));
                AddStats("armwrestling", (a1?.Count ?? 0) + (a2?.Count ?? 0), (a1?.Wins ?? 0) + (a2?.Wins ?? 0));
                AddStats("badminton", (b1?.Count ?? 0) + (b2?.Count ?? 0), (b1?.Wins ?? 0) + (b2?.Wins ?? 0));
                
                // 4. Team Sport Stats & Goals
                var pGoals = goalsS.FirstOrDefault(x => x.Id == playerId)?.Goals ?? 0;
                if (pGoals > 0) {
                    if (!sportStatsMap.ContainsKey("futsal")) sportStatsMap["futsal"] = new SportStatsDto { SportType = "futsal" };
                    var s = sportStatsMap["futsal"];
                    s.GoalsCount += pGoals;
                    int goalPts = pGoals * 5;
                    s.PerformanceScore += goalPts;
                    totalScore += goalPts;
                    
                    pointsBreakdown.Add(new PointEntryDto { 
                        Description = $"Забиті голи (Футзал): {pGoals} голів", 
                        Points = goalPts, 
                        Category = "Goals" 
                    });
                }

                dto.MatchesPlayed = played;
                dto.WinsCount = won;
                dto.GoalsScored = pGoals;
                dto.PerformanceScore = (int)Math.Round(totalScore);
                dto.SportStats = sportStatsMap.Values.ToList();
                dto.PointsBreakdown = pointsBreakdown.OrderByDescending(x => x.Points).ToList();
                playerDtos.Add(dto);
            }

            return playerDtos.OrderByDescending(p => p.PerformanceScore).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllWithRankingsAsync");
            try { System.IO.File.WriteAllText("rankings_error.txt", ex.ToString()); } catch {}
            return Enumerable.Empty<PlayerDto>();
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
                achievements.Add(new AchievementDto {
                    TournamentName = p.Tournament.Name,
                    SportType = "chess",
                    Date = p.Tournament.Date,
                    Rank = rank,
                    TournamentId = p.TournamentId,
                    Result = rank > 0 ? $"{rank} місце ({p.Points} очок)" : $"{p.Points} очок"
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
                achievements.Add(new AchievementDto {
                    TournamentName = p.Event.Name,
                    SportType = "checkers",
                    Date = p.Event.Date,
                    Rank = rank,
                    TournamentId = p.TournamentId,
                    Result = rank > 0 ? $"{rank} місце ({p.Points} очок)" : $"{p.Points} очок"
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
            var genericWinners = await _dbContext.Set<Event>()
                .Where(e => e.FirstPlaceId == playerId || e.SecondPlaceId == playerId || e.ThirdPlaceId == playerId)
                .ToListAsync(cancellationToken);

            foreach (var e in genericWinners) {
                if (achievements.Any(a => a.TournamentName == e.Name && a.Date == e.Date)) continue;
                int rank = e.FirstPlaceId == playerId ? 1 : (e.SecondPlaceId == playerId ? 2 : 3);
                string sport = e is ArmwrestlingEvent ? "armwrestling" : "individual";
                achievements.Add(new AchievementDto {
                    TournamentName = e.Name,
                    SportType = sport,
                    Date = e.Date,
                    Rank = rank,
                    TournamentId = e.Id,
                    Result = $"{rank} місце"
                });
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error populating Generic achievements for player {PlayerId}", playerId);
        }

        // 6. Team Sports (Futsal, Volleyball, etc.)
        // Futsal
        try {
            var teams = await _dbContext.FutsalTeams
                .Include(t => t.Event)
                .Where(t => t.Players != null && t.Players.Any(pl => pl.Id == playerId))
                .ToListAsync(cancellationToken);

            foreach (var team in teams) {
                if (team.Event == null) continue;
                int rank = 0;
                if (team.Id == team.Event.FirstPlaceTeamId) rank = 1;
                else if (team.Id == team.Event.SecondPlaceTeamId) rank = 2;
                else if (team.Id == team.Event.ThirdPlaceTeamId) rank = 3;
                else {
                    rank = await _dbContext.FutsalTeams
                        .Where(others => others.EventId == team.EventId && others.Points > team.Points)
                        .CountAsync(cancellationToken) + 1;
                }
                achievements.Add(new AchievementDto {
                    TournamentName = team.Event.Name,
                    SportType = "futsal",
                    Date = team.Event.Date,
                    Rank = rank,
                    TournamentId = team.EventId,
                    Result = rank > 0 ? $"{rank} місце (команда {team.Name})" : $"Учасник (команда {team.Name})"
                });
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error populating Futsal achievements for player {PlayerId}", playerId);
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
                int rank = (t.Id == t.Event.FirstPlaceTeamId) ? 1 : ((t.Id == t.Event.SecondPlaceTeamId) ? 2 : ((t.Id == t.Event.ThirdPlaceTeamId) ? 3 : 0));
                achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "volleyball", Date = t.Event.Date, Rank = rank, TournamentId = t.EventId, Result = rank > 0 ? $"{rank} місце ({t.Name})" : $"Учасник ({t.Name})" });
            }

            // Basketball 3x3
            var bTeams = await _dbContext.Basketball3x3Teams
                .Include(t => t.Event)
                .Where(t => t.Players != null && t.Players.Any(pl => pl.Id == playerId))
                .ToListAsync(cancellationToken);
            foreach (var t in bTeams) {
                if (t.Event == null) continue;
                int rank = (t.Id == t.Event.FirstPlaceTeamId) ? 1 : ((t.Id == t.Event.SecondPlaceTeamId) ? 2 : ((t.Id == t.Event.ThirdPlaceTeamId) ? 3 : 0));
                achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "basketball_3x3", Date = t.Event.Date, Rank = rank, TournamentId = t.EventId, Result = rank > 0 ? $"{rank} місце ({t.Name})" : $"Учасник ({t.Name})" });
            }

            // Beach Volleyball
            var bvTeams = await _dbContext.BeachVolleyballTeams
                .Include(t => t.Event)
                .Where(t => t.Players != null && t.Players.Any(pl => pl.Id == playerId))
                .ToListAsync(cancellationToken);
            foreach (var t in bvTeams) {
                if (t.Event == null) continue;
                int rank = (t.Id == t.Event.FirstPlaceTeamId) ? 1 : ((t.Id == t.Event.SecondPlaceTeamId) ? 2 : ((t.Id == t.Event.ThirdPlaceTeamId) ? 3 : 0));
                achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "beach_volleyball", Date = t.Event.Date, Rank = rank, TournamentId = t.EventId, Result = rank > 0 ? $"{rank} місце ({t.Name})" : $"Учасник ({t.Name})" });
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error populating other team achievements for player {PlayerId}", playerId);
        }

        dto.Achievements = achievements
            .OrderByDescending(a => a.Rank > 0 && a.Rank <= 3) // Podium first
            .ThenBy(a => a.Rank > 0 ? a.Rank : 999) // 1, 2, 3... then others
            .ThenByDescending(a => a.Date)
            .ToList();
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
            int pts = (p * 1) + (w * 2);
            totalScore += pts;
            pointsBreakdown.Add(new PointEntryDto { Description = $"Участь у матчах ({sportName}): {p} ігор", Points = p * 1, Category = "Activity" });
            if (w > 0) pointsBreakdown.Add(new PointEntryDto { Description = $"Перемоги ({sportName}): {w} ігор", Points = w * 2, Category = "Activity" });
        }

        AddActivityStats("Шахи", chessMatches, chessWins);
        AddActivityStats("Шашки", checkersMatches, checkersWins);
        AddActivityStats("Настільний теніс", ttMatches, ttWins);
        AddActivityStats("Армрестлинг", armMatches, armWins);
        AddActivityStats("Бадмінтон", badMatches, badWins);

        dto.MatchesPlayed = chessMatches + checkersMatches + ttMatches + armMatches + badMatches;
        dto.WinsCount = chessWins + checkersWins + ttWins + armWins + badWins;

        // 3. Futsal Goals
        int futsalGoals = await _dbContext.FutsalGoals
            .Where(g => g.PlayerId == playerId)
            .SumAsync(g => (int?)g.GoalsCount ?? 0, cancellationToken);
        
        if (futsalGoals > 0) {
            int goalPts = futsalGoals * 5;
            totalScore += goalPts;
            pointsBreakdown.Add(new PointEntryDto { 
                Description = $"Забиті голи (Футзал): {futsalGoals} голів", 
                Points = goalPts, 
                Category = "Goals" 
            });
        }
        
        dto.GoalsScored = futsalGoals;
        dto.PerformanceScore = (int)Math.Round(totalScore);
        dto.PointsBreakdown = pointsBreakdown.OrderByDescending(x => x.Points).ToList();
    }
}
