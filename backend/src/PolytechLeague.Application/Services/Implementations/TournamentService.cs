using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Futsal;
using PolytechLeague.Domain.Entities.Basketball3x3;
using PolytechLeague.Domain.Entities.Volleyball;
using PolytechLeague.Domain.Entities.Chess;
using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Domain.Entities.TableTennis;
using PolytechLeague.Domain.Entities.Armwrestling;
using PolytechLeague.Domain.Entities.Badminton;
using PolytechLeague.Domain.Entities.BeachVolleyball;

namespace PolytechLeague.Application.Services.Implementations;

public class TournamentService : ITournamentService
{
    private readonly ApplicationDbContext _context;

    public TournamentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<TournamentDto>> GetAllTournamentsAsync(
        int page = 1, 
        int pageSize = 10,
        string? status = null,
        string? sport = null,
        string? level = null)

    {
        var query = _context.Set<PolytechLeague.Domain.Entities.Event>()
            .Include(e => e.Photos)
            .AsQueryable();

        // Server-side filtering by level
        if (!string.IsNullOrEmpty(level) && level != "all")
        {
            if (Enum.TryParse<PolytechLeague.Domain.Entities.CompetitionLevel>(level, true, out var levelEnum))
            {
                query = query.Where(e => e.Level == levelEnum);
            }
        }


        // Server-side filtering by sport
        if (!string.IsNullOrEmpty(sport) && sport != "all")
        {
            query = query.Where(e =>
                (sport == "futsal" && e is FutsalEvent) ||
                (sport == "basketball-3x3" && e is Basketball3x3Event) ||
                (sport == "volleyball" && e is VolleyballEvent) ||
                (sport == "chess" && e is ChessEvent) ||
                (sport == "checkers" && e is CheckersEvent) ||
                (sport == "table-tennis" && e is TableTennisEvent) ||
                (sport == "armwrestling" && e is ArmwrestlingEvent) ||
                (sport == "badminton" && e is BadmintonEvent) ||
                (sport == "beach-volleyball" && e is BeachVolleyballEvent)
            );
        }

        // Server-side filtering by status (based on date logic)
        if (!string.IsNullOrEmpty(status) && status != "all")
        {
            var today = DateTime.UtcNow.Date;
            if (status == "upcoming")
            {
                query = query.Where(e => e.Date > today);
            }
            else if (status == "ongoing")
            {
                query = query.Where(e => e.Date == today);
            }
            else if (status == "completed")
            {
                query = query.Where(e => e.Date < today);
            }
        }

        var totalCount = await query.CountAsync();

        var events = await query
            .OrderByDescending(e => e.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var tournamentDtos = events.Select(e => new TournamentDto
        {
            Id = e.Id,
            Title = e.Name,
            Sport = GetSportName(e),
            Status = GetStatus(e),
            Date = e.Date.ToString("dd.MM.yyyy"),
            Location = e.Location,
            Description = e.Description ?? string.Empty,
            Level = GetLevelName(e.Level),
            Category = GetCategoryName(e.Category),
            Participants = 0,
            Teams = 0,
            Prize = "Призи та кубки",
            Organizer = "Спортвідділ",
            RegistrationStart = e.RegistrationStart?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            RegistrationEnd = e.RegistrationEnd?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? e.Date.AddDays(-5).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            EndDate = e.EndDate?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            CurrentRound = GetStatus(e) == "ongoing" ? "В процесі" : "Реєстрація",
            Registered = false,
            Featured = false,
            IsTeam = e.IsTeam,
            IsModerated = e.IsModerated,
            IsPublic = e.IsPublic,
            SeasonId = e.SeasonId,
            MainPhotoUrl = e.MainPhotoUrl,
            Time = e.Date.ToString("HH:mm"),
            RawDate = e.Date
        }).ToList();

        foreach (var dto in tournamentDtos)
        {
            await PopulateCountsAsync(dto);
        }

        return new PagedResponse<TournamentDto>
        {
            Items = tournamentDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TournamentDto?> GetTournamentByIdAsync(long id)
    {
        var e = await _context.Set<PolytechLeague.Domain.Entities.Event>()
            .Include(e => e.Photos)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (e == null) return null;

        var dto = new TournamentDto
        {
            Id = e.Id,
            Title = e.Name,
            Sport = GetSportName(e),
            Status = GetStatus(e),
            Date = e.Date.ToString("dd MMMM, yyyy", new System.Globalization.CultureInfo("uk-UA")),
            Location = e.Location,
            Description = e.Description ?? string.Empty,
            Level = GetLevelName(e.Level),
            Category = GetCategoryName(e.Category),
            Participants = 0, // Need to implement counting
            Teams = 0, // Need to implement counting
            Prize = "Призи та кубки",
            Organizer = "Спортвідділ",
            RegistrationStart = e.RegistrationStart?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            RegistrationEnd = e.RegistrationEnd?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? e.Date.AddDays(-5).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            EndDate = e.EndDate?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            CurrentRound = GetStatus(e) == "ongoing" ? "В процесі" : "Реєстрація",
            Registered = false,
            Featured = false,
            IsTeam = e.IsTeam,
            IsModerated = e.IsModerated,
            IsPublic = e.IsPublic,
            SeasonId = e.SeasonId,
            MainPhotoUrl = e.MainPhotoUrl,
            Time = e.Date.ToString("HH:mm"),
            RawDate = e.Date
        };

        await PopulateCountsAsync(dto);
        return dto;
    }

    private async Task PopulateCountsAsync(TournamentDto dto)
    {
        switch (dto.Sport)
        {
            case "futsal":
                dto.Teams = await _context.FutsalTeams.CountAsync(t => t.EventId == dto.Id);
                dto.Participants = dto.Teams; // Typically show team count for team sports
                break;
            case "basketball-3x3":
                dto.Teams = await _context.Basketball3x3Teams.CountAsync(t => t.EventId == dto.Id);
                dto.Participants = dto.Teams;
                break;
            case "volleyball":
                dto.Teams = await _context.VolleyballTeams.CountAsync(t => t.EventId == dto.Id);
                dto.Participants = dto.Teams;
                break;
            case "beach-volleyball":
                dto.Teams = await _context.BeachVolleyballTeams.CountAsync(t => t.EventId == dto.Id);
                dto.Participants = dto.Teams;
                break;
            case "chess":
                if (dto.IsTeam)
                {
                    dto.Teams = await _context.ChessTeams.CountAsync(t => t.EventId == dto.Id);
                    dto.Participants = dto.Teams;
                }
                else
                {
                    dto.Participants = await _context.ChessEventParticipants.CountAsync(p => p.TournamentId == dto.Id);
                }
                break;
            case "checkers":
                if (dto.IsTeam)
                {
                    dto.Teams = await _context.CheckersTeams.CountAsync(t => t.EventId == dto.Id);
                    dto.Participants = dto.Teams;
                }
                else
                {
                    dto.Participants = await _context.CheckersEventParticipants.CountAsync(p => p.TournamentId == dto.Id);
                }
                break;
            case "table-tennis":
                dto.Participants = await _context.TableTennisEventParticipants.CountAsync(p => p.TournamentId == dto.Id);
                break;
            case "badminton":
                if (dto.IsTeam)
                {
                    dto.Teams = await _context.BadmintonTeams.CountAsync(t => t.EventId == dto.Id);
                    dto.Participants = dto.Teams;
                }
                else
                {
                    dto.Participants = await _context.BadmintonEventParticipants.CountAsync(p => p.EventId == dto.Id);
                }
                break;
            case "armwrestling":
                var armEvent = await _context.ArmwrestlingEvents.FirstOrDefaultAsync(e => e.Id == dto.Id);
                if (armEvent != null)
                {
                    dto.Participants = armEvent.ParticipantsCount;
                }
                break;
        }
    }

    private string GetSportName(PolytechLeague.Domain.Entities.Event e)
    {
        return e switch
        {
            FutsalEvent => "futsal",
            Basketball3x3Event => "basketball-3x3",
            VolleyballEvent => "volleyball",
            BeachVolleyballEvent => "beach-volleyball",
            ChessEvent => "chess",
            CheckersEvent => "checkers",
            TableTennisEvent => "table-tennis",
            ArmwrestlingEvent => "armwrestling",
            BadmintonEvent => "badminton",
            _ => "other"
        };
    }

    private string GetStatus(PolytechLeague.Domain.Entities.Event e)
    {
        var now = DateTime.UtcNow;

        // 1. Completed: if EndDate passed, or if no EndDate and more than 1 day after start
        if (e.EndDate.HasValue && e.EndDate.Value < now) return "completed";
        if (!e.EndDate.HasValue && e.Date < now.AddDays(-1)) return "completed";

        // 2. Ongoing: if currently between start and end dates
        if (e.Date <= now && (e.EndDate.HasValue ? e.EndDate.Value >= now : e.Date.AddDays(1) >= now))
            return "ongoing";

        // 3. Registration: if within registration period
        if (e.RegistrationStart.HasValue && e.RegistrationStart.Value <= now && 
            e.RegistrationEnd.HasValue && e.RegistrationEnd.Value >= now)
            return "registration";

        // 4. Upcoming: if in the future
        if (e.Date > now) return "upcoming";

        return "completed";
    }

    public async Task<GeneralStatsDto> GetGeneralStatsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var startOfSeason = new DateTime(today.Year, 9, 1);
        if (today.Month < 9) startOfSeason = new DateTime(today.Year - 1, 9, 1);
        
        var activeTournamentsCount = await _context.Set<PolytechLeague.Domain.Entities.Event>()
            .CountAsync(e => e.Date >= startOfSeason);

        // Count only players who are in the ranking (have at least one achievement or participation)
        var rankedPlayersCount = await _context.Players
            .Where(p => p.User == null || p.User.Role != UserRole.Admin)
            .Where(p => !p.IsExternal && !p.IsGuest)
            .CountAsync(p => _context.Set<Event>().Any(e => e.FirstPlaceId == p.Id || e.SecondPlaceId == p.Id || e.ThirdPlaceId == p.Id) ||
                            _context.FutsalEventParticipants.Any(fp => fp.PlayerId == p.Id) ||
                            _context.VolleyballEventParticipants.Any(vp => vp.PlayerId == p.Id) ||
                            _context.Basketball3x3EventParticipants.Any(bp => bp.PlayerId == p.Id) ||
                            _context.ChessEventParticipants.Any(cp => cp.PlayerId == p.Id) ||
                            _context.CheckersEventParticipants.Any(cp => cp.PlayerId == p.Id) ||
                            _context.BeachVolleyballEventParticipants.Any(bvp => bvp.PlayerId == p.Id) ||
                            _context.TableTennisEventParticipants.Any(ttp => ttp.PlayerId == p.Id) ||
                            _context.BadmintonEventParticipants.Any(bmp => bmp.PlayerId == p.Id) ||
                            _context.ArmwrestlingEventParticipants.Any(ap => ap.PlayerId == p.Id));

        var registeredPlayersCount = rankedPlayersCount;

        // Count total matches across all sport tables
        int totalMatches = 0;
        totalMatches += await _context.FutsalMatches.CountAsync();
        totalMatches += await _context.VolleyballMatches.CountAsync();
        totalMatches += await _context.Basketball3x3Matches.CountAsync();
        totalMatches += await _context.ChessGames.CountAsync();
        totalMatches += await _context.CheckersGames.CountAsync();
        totalMatches += await _context.BeachVolleyballMatches.CountAsync();
        totalMatches += await _context.TableTennisMatches.CountAsync();
        totalMatches += await _context.BadmintonMatches.CountAsync();
        totalMatches += await _context.ArmwrestlingMatches.CountAsync();

        // Calculate Season Progress (Sept 1st to June 30th)
        int progress = 0;
        var start = new DateTime(today.Year, 9, 1);
        if (today.Month < 9) start = new DateTime(today.Year - 1, 9, 1);
        var end = start.AddMonths(10); // Ends end of June next year
        
        var totalDays = (end - start).TotalDays;
        var elapsedDays = (today - start).TotalDays;
        
        if (elapsedDays > 0 && totalDays > 0)
        {
            progress = (int)Math.Min(100, Math.Max(0, (elapsedDays / totalDays) * 100));
        }

        return new GeneralStatsDto
        {
            ActiveTournamentsCount = activeTournamentsCount,
            RegisteredPlayersCount = registeredPlayersCount,
            TotalMatchesCount = totalMatches,
            SeasonProgress = progress,
            TopSport = "Футзал" // Could be dynamic based on match counts
        };
    }

    public async Task<IEnumerable<RecentMatchDto>> GetRecentMatchesAsync(int count, string? sport = null)
    {
        // Normalize sport for comparison
        if (sport != null) sport = sport.ToLower();

        var allMatches = new List<RecentMatchDto>();

        // 1. Futsal
        if (sport == null || sport == "futsal")
        {
            var futsalMatches = await _context.FutsalMatches
                .Include(m => m.Event)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.HomeTeamScore != null && m.AwayTeamScore != null)
                .OrderByDescending(m => m.Event!.Date)
                .Take(count)
                .Select(m => new RecentMatchDto
                {
                    Id = m.Id,
                    Sport = "futsal",
                    Team1Name = m.HomeTeam!.Name,
                    Team2Name = m.AwayTeam!.Name,
                    Score1 = m.HomeTeamScore.ToString(),
                    Score2 = m.AwayTeamScore.ToString(),
                    Status = "Completed",
                    Date = m.Event!.Date.ToString("dd.MM.yyyy"),
                    Location = m.Event.Location,
                    TournamentName = m.Event.Name,
                    TournamentId = m.Event.Id
                })
                .ToListAsync();
            allMatches.AddRange(futsalMatches);
        }

        // 2. Volleyball
        if (sport == null || sport == "volleyball")
        {
            var volleyballMatches = await _context.VolleyballMatches
                .Include(m => m.Event)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.HomeSetsWon != null && m.AwaySetsWon != null)
                .OrderByDescending(m => m.Event!.Date)
                .Take(count)
                .Select(m => new RecentMatchDto
                {
                    Id = m.Id,
                    Sport = "volleyball",
                    Team1Name = m.HomeTeam!.Name,
                    Team2Name = m.AwayTeam!.Name,
                    Score1 = m.HomeSetsWon.ToString(),
                    Score2 = m.AwaySetsWon.ToString(),
                    Status = "Completed",
                    Date = m.Event!.Date.ToString("dd.MM.yyyy"),
                    Location = m.Event.Location,
                    TournamentName = m.Event.Name,
                    TournamentId = m.Event.Id
                })
                .ToListAsync();
            allMatches.AddRange(volleyballMatches);
        }

        // 3. Basketball 3x3
        if (sport == null || sport == "basketball-3x3")
        {
            var basketballMatches = await _context.Basketball3x3Matches
                .Include(m => m.Event)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.HomeTeamScore != null && m.AwayTeamScore != null)
                .OrderByDescending(m => m.Event!.Date)
                .Take(count)
                .Select(m => new RecentMatchDto
                {
                    Id = m.Id,
                    Sport = "basketball-3x3",
                    Team1Name = m.HomeTeam!.Name,
                    Team2Name = m.AwayTeam!.Name,
                    Score1 = m.HomeTeamScore.ToString(),
                    Score2 = m.AwayTeamScore.ToString(),
                    Status = "Completed",
                    Date = m.Event!.Date.ToString("dd.MM.yyyy"),
                    Location = m.Event.Location,
                    TournamentName = m.Event.Name,
                    TournamentId = m.Event.Id
                })
                .ToListAsync();
            allMatches.AddRange(basketballMatches);
        }

        // 4. Chess
        if (sport == null || sport == "chess")
        {
            var chessGames = await _context.ChessGames
                .Include(m => m.Tournament)
                .Include(m => m.WhitePlayer)
                .Include(m => m.BlackPlayer)
                .Where(m => m.Result != PolytechLeague.Domain.Entities.Shared.GameResult.NotPlayed)
                .OrderByDescending(m => m.Tournament!.Date)
                .Take(count)
                .Select(m => new RecentMatchDto
                {
                    Id = m.Id,
                    Sport = "chess",
                    Team1Name = m.WhitePlayer != null ? $"{m.WhitePlayer.FirstName} {m.WhitePlayer.LastName}" : "TBD",
                    Team2Name = m.BlackPlayer != null ? $"{m.BlackPlayer.FirstName} {m.BlackPlayer.LastName}" : "TBD",
                    Score1 = m.Result == PolytechLeague.Domain.Entities.Shared.GameResult.WhiteWin ? "1" : (m.Result == PolytechLeague.Domain.Entities.Shared.GameResult.Draw ? "0.5" : "0"),
                    Score2 = m.Result == PolytechLeague.Domain.Entities.Shared.GameResult.BlackWin ? "1" : (m.Result == PolytechLeague.Domain.Entities.Shared.GameResult.Draw ? "0.5" : "0"),
                    Status = "Completed",
                    Date = m.Tournament!.Date.ToString("dd.MM.yyyy"),
                    Location = m.Tournament.Location,
                    TournamentName = m.Tournament.Name,
                    TournamentId = m.Tournament.Id
                })
                .ToListAsync();
            allMatches.AddRange(chessGames);
        }

        // 5. Checkers
        if (sport == null || sport == "checkers")
        {
            var checkersGames = await _context.CheckersGames
                .Include(m => m.Event)
                .Include(m => m.WhitePlayer).ThenInclude(p => p.Player)
                .Include(m => m.BlackPlayer).ThenInclude(p => p.Player)
                .Where(m => m.Result != PolytechLeague.Domain.Entities.Shared.GameResult.NotPlayed)
                .OrderByDescending(m => m.Event.Date)
                .Take(count)
                .Select(m => new RecentMatchDto
                {
                    Id = m.Id,
                    Sport = "checkers",
                    Team1Name = m.WhitePlayer != null && m.WhitePlayer.Player != null ? $"{m.WhitePlayer.Player.FirstName} {m.WhitePlayer.Player.LastName}" : "TBD",
                    Team2Name = m.BlackPlayer != null && m.BlackPlayer.Player != null ? $"{m.BlackPlayer.Player.FirstName} {m.BlackPlayer.Player.LastName}" : "TBD",
                    Score1 = m.Result == PolytechLeague.Domain.Entities.Shared.GameResult.WhiteWin ? "2" : (m.Result == PolytechLeague.Domain.Entities.Shared.GameResult.Draw ? "1" : "0"),
                    Score2 = m.Result == PolytechLeague.Domain.Entities.Shared.GameResult.BlackWin ? "2" : (m.Result == PolytechLeague.Domain.Entities.Shared.GameResult.Draw ? "1" : "0"),
                    Status = "Completed",
                    Date = m.Event.Date.ToString("dd.MM.yyyy"),
                    Location = m.Event.Location,
                    TournamentName = m.Event.Name,
                    TournamentId = m.Event.Id
                })
                .ToListAsync();
            allMatches.AddRange(checkersGames);
        }

        // 6. Beach Volleyball
        if (sport == null || sport == "beach-volleyball")
        {
            var beachVolleyballMatches = await _context.BeachVolleyballMatches
                .Include(m => m.Event)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.HomeSetsWon != null && m.AwaySetsWon != null)
                .OrderByDescending(m => m.Event!.Date)
                .Take(count)
                .Select(m => new RecentMatchDto
                {
                    Id = m.Id,
                    Sport = "beach-volleyball",
                    Team1Name = m.HomeTeam!.Name,
                    Team2Name = m.AwayTeam!.Name,
                    Score1 = m.HomeSetsWon.ToString(),
                    Score2 = m.AwaySetsWon.ToString(),
                    Status = "Completed",
                    Date = m.Event!.Date.ToString("dd.MM.yyyy"),
                    Location = m.Event.Location,
                    TournamentName = m.Event.Name,
                    TournamentId = m.Event.Id
                })
                .ToListAsync();
            allMatches.AddRange(beachVolleyballMatches);
        }

        // 7. Table Tennis
        if (sport == null || sport == "table-tennis")
        {
            var tableTennisMatches = await _context.TableTennisMatches
                .Include(m => m.Tournament)
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Where(m => m.WinnerId != null)
                .OrderByDescending(m => m.Tournament!.Date)
                .Take(count)
                .Select(m => new RecentMatchDto
                {
                    Id = m.Id,
                    Sport = "table-tennis",
                    Team1Name = m.Player1 != null ? $"{m.Player1.FirstName} {m.Player1.LastName}" : "TBD",
                    Team2Name = m.Player2 != null ? $"{m.Player2.FirstName} {m.Player2.LastName}" : "TBD",
                    Score1 = m.WinnerId == m.Player1Id ? "1" : "0",
                    Score2 = m.WinnerId == m.Player2Id ? "1" : "0",
                    Status = "Completed",
                    Date = m.Tournament!.Date.ToString("dd.MM.yyyy"),
                    Location = m.Tournament.Location,
                    TournamentName = m.Tournament.Name,
                    TournamentId = m.Tournament.Id
                })
                .ToListAsync();
            allMatches.AddRange(tableTennisMatches);
        }

        // 8. Badminton
        if (sport == null || sport == "badminton")
        {
            var badmintonMatches = await _context.BadmintonMatches
                .Include(m => m.Event)
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Where(m => m.HomeScore != null && m.AwayScore != null)
                .OrderByDescending(m => m.Event!.Date)
                .Take(count)
                .Select(m => new RecentMatchDto
                {
                    Id = m.Id,
                    Sport = "badminton",
                    Team1Name = m.Player1 != null ? $"{m.Player1.FirstName} {m.Player1.LastName}" : (m.HomeTeam != null ? m.HomeTeam.Name : "TBD"),
                    Team2Name = m.Player2 != null ? $"{m.Player2.FirstName} {m.Player2.LastName}" : (m.AwayTeam != null ? m.AwayTeam.Name : "TBD"),
                    Score1 = m.HomeScore.ToString(),
                    Score2 = m.AwayScore.ToString(),
                    Status = "Completed",
                    Date = m.Event!.Date.ToString("dd.MM.yyyy"),
                    Location = m.Event.Location,
                    TournamentName = m.Event.Name,
                    TournamentId = m.Event.Id
                })
                .ToListAsync();
            allMatches.AddRange(badmintonMatches);
        }

        return allMatches
            .OrderByDescending(r => DateTime.ParseExact(r.Date, "dd.MM.yyyy", null))
            .Take(count)
            .ToList();
    }

    private string GetLevelName(CompetitionLevel level)
    {
        return level switch
        {
            CompetitionLevel.CollegeGroup => "CollegeGroup",
            CompetitionLevel.Regional => "Regional",
            CompetitionLevel.National => "National",
            _ => "CollegeGroup"
        };
    }

    private string GetCategoryName(EventCategory category)
    {
        return category switch
        {
            EventCategory.Men => "Men",
            EventCategory.Women => "Women",
            EventCategory.Mixed => "Mixed",
            _ => "Mixed"
        };
    }
    public async Task<IEnumerable<PolytechLeague.Application.Dto.General.ChampionDto>> GetChampionsAsync(string? sportType = null)
    {
        var query = _context.Set<PolytechLeague.Domain.Entities.Event>()
            .Where(e => e.Level == CompetitionLevel.CollegeGroup && e.FirstPlaceId != null)
            .Include(e => e.Season)
            .Include(e => e.FirstPlace)
            .AsQueryable();

        if (!string.IsNullOrEmpty(sportType) && sportType != "all")
        {
            query = query.Where(e =>
                (sportType == "chess" && e is ChessEvent) ||
                (sportType == "checkers" && e is CheckersEvent) ||
                (sportType == "table-tennis" && e is TableTennisEvent) ||
                (sportType == "armwrestling" && e is ArmwrestlingEvent) ||
                (sportType == "badminton" && e is BadmintonEvent) ||
                (sportType == "futsal" && e is FutsalEvent) ||
                (sportType == "basketball-3x3" && e is Basketball3x3Event) ||
                (sportType == "volleyball" && e is VolleyballEvent) ||
                (sportType == "beach-volleyball" && e is BeachVolleyballEvent)
            );
        }

        var results = await query
            .OrderByDescending(e => e.Date)
            .ToListAsync();

        return results.Select(e => new PolytechLeague.Application.Dto.General.ChampionDto
        {
            PlayerId = e.FirstPlaceId!.Value,
            PlayerName = $"{e.FirstPlace!.FirstName} {e.FirstPlace.LastName}",
            PlayerPhotoUrl = e.FirstPlace.PhotoUrl,
            SeasonName = e.Season?.Name ?? e.Date.Year.ToString(),
            TournamentId = e.Id,
            TournamentName = e.Name,
            SportType = GetSportName(e),
            Date = e.Date,
            Points = 0 
        });
    }
}
