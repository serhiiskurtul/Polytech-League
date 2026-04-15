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
        player.LastName = dto.LastName ?? player.LastName;
        player.Group = dto.Group ?? player.Group;
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
                        p.LastName.ToLower().Contains(lowerQuery) ||
                        p.Group.ToLower().Contains(lowerQuery))
            .ToListAsync(cancellationToken);
        
        return players.Select(p => p.ToDto());
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
                achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "volleyball", Date = t.Event.Date, Rank = rank, Result = rank > 0 ? $"{rank} місце ({t.Name})" : $"Учасник ({t.Name})" });
            }

            // Basketball 3x3
            var bTeams = await _dbContext.Basketball3x3Teams
                .Include(t => t.Event)
                .Where(t => t.Players != null && t.Players.Any(pl => pl.Id == playerId))
                .ToListAsync(cancellationToken);
            foreach (var t in bTeams) {
                if (t.Event == null) continue;
                int rank = (t.Id == t.Event.FirstPlaceTeamId) ? 1 : ((t.Id == t.Event.SecondPlaceTeamId) ? 2 : ((t.Id == t.Event.ThirdPlaceTeamId) ? 3 : 0));
                achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "basketball_3x3", Date = t.Event.Date, Rank = rank, Result = rank > 0 ? $"{rank} місце ({t.Name})" : $"Учасник ({t.Name})" });
            }

            // Beach Volleyball
            var bvTeams = await _dbContext.BeachVolleyballTeams
                .Include(t => t.Event)
                .Where(t => t.Players != null && t.Players.Any(pl => pl.Id == playerId))
                .ToListAsync(cancellationToken);
            foreach (var t in bvTeams) {
                if (t.Event == null) continue;
                int rank = (t.Id == t.Event.FirstPlaceTeamId) ? 1 : ((t.Id == t.Event.SecondPlaceTeamId) ? 2 : ((t.Id == t.Event.ThirdPlaceTeamId) ? 3 : 0));
                achievements.Add(new AchievementDto { TournamentName = t.Event.Name, SportType = "beach_volleyball", Date = t.Event.Date, Rank = rank, Result = rank > 0 ? $"{rank} місце ({t.Name})" : $"Учасник ({t.Name})" });
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

        // 1. Matches Played
        int chessMatches = await _dbContext.ChessGames.CountAsync(g => g.WhitePlayerId == playerId || g.BlackPlayerId == playerId, cancellationToken);
        int checkersMatches = await _dbContext.CheckersGames.CountAsync(g => g.WhitePlayerId == playerId || g.BlackPlayerId == playerId, cancellationToken);
        int ttMatches = await _dbContext.TableTennisMatches.CountAsync(g => g.Player1Id == playerId || g.Player2Id == playerId, cancellationToken);
        int armMatches = await _dbContext.ArmwrestlingMatches.CountAsync(g => g.Player1Id == playerId || g.Player2Id == playerId, cancellationToken);
        
        dto.MatchesPlayed = chessMatches + checkersMatches + ttMatches + armMatches;

        // 2. Wins Count
        int chessWins = await _dbContext.ChessGames.CountAsync(g => 
            (g.WhitePlayerId == playerId && g.Result == GameResult.WhiteWin) || 
            (g.BlackPlayerId == playerId && g.Result == GameResult.BlackWin), cancellationToken);
            
        int checkersWins = await _dbContext.CheckersGames.CountAsync(g => 
            (g.WhitePlayerId == playerId && g.Result == GameResult.WhiteWin) || 
            (g.BlackPlayerId == playerId && g.Result == GameResult.BlackWin), cancellationToken);
            
        int ttWins = await _dbContext.TableTennisMatches.CountAsync(g => g.WinnerId == playerId, cancellationToken);
        int armWins = await _dbContext.ArmwrestlingMatches.CountAsync(g => g.WinnerId == playerId, cancellationToken);

        dto.WinsCount = chessWins + checkersWins + ttWins + armWins;

        // 3. Goals (TBD - placeholders for now as we don't have individual goal tracking yet)
        dto.GoalsScored = 0;
    }
}
