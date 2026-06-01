using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Football;
using PolytechLeague.Domain.Entities.Basketball;
using PolytechLeague.Domain.Entities.Volleyball;
using PolytechLeague.Domain.Entities.Chess;
using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Domain.Entities.TableTennis;
using PolytechLeague.Domain.Entities.Armwrestling;
using PolytechLeague.Domain.Entities.Badminton;
using PolytechLeague.Domain.Entities.BeachVolleyball;
using PolytechLeague.Domain.Entities.Athletics;
using PolytechLeague.Domain.Entities.Enums;

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
                (sport == "football" && e is FootballEvent) ||
                (sport == "basketball-3x3" && e is BasketballEvent) ||
                (sport == "volleyball" && e is VolleyballEvent) ||
                (sport == "chess" && e is ChessEvent) ||
                (sport == "checkers" && e is CheckersEvent) ||
                (sport == "table-tennis" && e is TableTennisEvent) ||
                (sport == "armwrestling" && e is ArmwrestlingEvent) ||
                (sport == "badminton" && e is BadmintonEvent) ||
                (sport == "beach-volleyball" && e is BeachVolleyballEvent) ||
                (sport == "athletics" && e is AthleticsEvent)
            );
        }

        // Server-side filtering by status (based on date logic)
        if (!string.IsNullOrEmpty(status) && status != "all")
        {
            var now = DateTime.UtcNow;
            if (status == "upcoming")
            {
                query = query.Where(e => e.Date > now);
            }
            else if (status == "ongoing")
            {
                query = query.Where(e => e.Date <= now && (e.EndDate.HasValue ? e.EndDate.Value >= now : e.Date >= now.AddDays(-1)));
            }
            else if (status == "completed")
            {
                query = query.Where(e => (e.EndDate.HasValue && e.EndDate.Value < now) || (!e.EndDate.HasValue && e.Date < now.AddDays(-1)));
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
            CurrentRound = GetCurrentRoundLabel(GetStatus(e)),
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
            .AsSplitQuery()
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
            CurrentRound = GetCurrentRoundLabel(GetStatus(e)),
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
            case "football":
                dto.Teams = await _context.FootballTeams.CountAsync(t => t.EventId == dto.Id);
                dto.Participants = dto.Teams; // Typically show team count for team sports
                break;
            case "basketball-3x3":
                dto.Teams = await _context.BasketballTeams.CountAsync(t => t.EventId == dto.Id);
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
            case "athletics":
                dto.Participants = await _context.AthleticsResults
                    .Where(r => r.AthleticsEventId == dto.Id)
                    .Select(r => r.PlayerId)
                    .Where(id => id != null)
                    .Distinct()
                    .CountAsync();
                break;
        }
    }

    private string GetSportName(PolytechLeague.Domain.Entities.Event e)
    {
        return e switch
        {
            FootballEvent => "football",
            BasketballEvent => "basketball-3x3",
            VolleyballEvent => "volleyball",
            BeachVolleyballEvent => "beach-volleyball",
            ChessEvent => "chess",
            CheckersEvent => "checkers",
            TableTennisEvent => "table-tennis",
            ArmwrestlingEvent => "armwrestling",
            BadmintonEvent => "badminton",
            AthleticsEvent => "athletics",
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

    private string GetCurrentRoundLabel(string status)
    {
        return status switch
        {
            "ongoing" => "В процесі",
            "completed" => "Завершено",
            "registration" => "Реєстрація",
            "upcoming" => "Заплановано",
            _ => "Реєстрація"
        };
    }

    public async Task<GeneralStatsDto> GetGeneralStatsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var startOfSeason = new DateTime(today.Year, 9, 1);
        if (today.Month < 9) startOfSeason = new DateTime(today.Year - 1, 9, 1);
        
        var activeTournamentsCount = await _context.Set<PolytechLeague.Domain.Entities.Event>()
            .CountAsync(e => e.Date >= startOfSeason);

        var eligibleEventIds = await _context.Set<PolytechLeague.Domain.Entities.Event>()
            .Where(e => e.Date >= startOfSeason)
            .Select(e => e.Id)
            .ToListAsync();

        // Count only players who are in the ranking (have at least one achievement or participation in the current season)
        var rankedPlayersCount = await _context.Players
            .Where(p => p.User == null || p.User.Role != UserRole.Admin)
            .Where(p => !p.IsExternal && !p.IsGuest)
            .CountAsync(p => _context.Set<Event>().Any(e => eligibleEventIds.Contains(e.Id) && (e.FirstPlaceId == p.Id || e.SecondPlaceId == p.Id || e.ThirdPlaceId == p.Id)) ||
                            _context.FootballEventParticipants.Any(fp => fp.PlayerId == p.Id && eligibleEventIds.Contains(fp.EventId)) ||
                            _context.VolleyballEventParticipants.Any(vp => vp.PlayerId == p.Id && eligibleEventIds.Contains(vp.EventId)) ||
                            _context.BasketballEventParticipants.Any(bp => bp.PlayerId == p.Id && eligibleEventIds.Contains(bp.EventId)) ||
                            _context.ChessEventParticipants.Any(cp => cp.PlayerId == p.Id && eligibleEventIds.Contains(cp.TournamentId)) ||
                            _context.CheckersEventParticipants.Any(cp => cp.PlayerId == p.Id && eligibleEventIds.Contains(cp.TournamentId)) ||
                            _context.BeachVolleyballEventParticipants.Any(bvp => bvp.PlayerId == p.Id && eligibleEventIds.Contains(bvp.EventId)) ||
                            _context.TableTennisEventParticipants.Any(ttp => ttp.PlayerId == p.Id && eligibleEventIds.Contains(ttp.TournamentId)) ||
                            _context.BadmintonEventParticipants.Any(bmp => bmp.PlayerId == p.Id && eligibleEventIds.Contains(bmp.EventId)) ||
                            _context.AthleticsResults.Any(ar => ar.PlayerId == p.Id && eligibleEventIds.Contains(ar.AthleticsEventId)) ||
                            _context.ArmwrestlingEventParticipants.Any(ap => ap.PlayerId == p.Id && eligibleEventIds.Contains(ap.TournamentId)));

        var registeredPlayersCount = rankedPlayersCount;

        // Count total matches across all sport tables
        int totalMatches = 0;
        totalMatches += await _context.FootballMatches.CountAsync();
        totalMatches += await _context.VolleyballMatches.CountAsync();
        totalMatches += await _context.BasketballMatches.CountAsync();
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
            TopSport = "Футбол" // Could be dynamic based on match counts
        };
    }

    public async Task<IEnumerable<RecentMatchDto>> GetRecentMatchesAsync(int count, string? sport = null)
    {
        // Normalize sport for comparison
        if (sport != null) sport = sport.ToLower();

        var allMatches = new List<RecentMatchDto>();

        // 1. Football
        if (sport == null || sport == "football")
        {
            var footballMatches = await _context.FootballMatches
                .Include(m => m.Event)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.HomeTeamScore != null && m.AwayTeamScore != null)
                .OrderByDescending(m => m.Event!.Date)
                .Take(count)
                .Select(m => new RecentMatchDto
                {
                    Id = m.Id,
                    Sport = "football",
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
            allMatches.AddRange(footballMatches);
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
            var basketballMatches = await _context.BasketballMatches
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
                    Team1Name = m.Player1 != null ? $"{m.Player1.FirstName} {m.Player1.LastName}" : (m.HomeTeam != null ? (m.HomeTeam.Name ?? "TBD") : "TBD"),
                    Team2Name = m.Player2 != null ? $"{m.Player2.FirstName} {m.Player2.LastName}" : (m.AwayTeam != null ? (m.AwayTeam.Name ?? "TBD") : "TBD"),
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
    public async Task<IEnumerable<PolytechLeague.Application.Dto.General.ChampionDto>> GetChampionsAsync(string? sportType = null, PolytechLeague.Domain.Entities.Enums.Gender? gender = null, string? weightClass = null)
    {
        var query = _context.Set<PolytechLeague.Domain.Entities.Event>()
            .Where(e => e.Level == CompetitionLevel.CollegeGroup)
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
                (sportType == "football" && e is FootballEvent) ||
                (sportType == "basketball-3x3" && e is BasketballEvent) ||
                (sportType == "volleyball" && e is VolleyballEvent) ||
                (sportType == "beach-volleyball" && e is BeachVolleyballEvent) ||
                (sportType == "athletics" && e is AthleticsEvent)
            );
        }

        if (gender.HasValue)
        {
            query = query.Where(e => e.FirstPlace!.Gender == gender.Value);
        }

        if (!string.IsNullOrEmpty(weightClass) && weightClass != "all")
        {
            if (Enum.TryParse<PolytechLeague.Domain.Entities.Armwrestling.Enums.ArmwrestlingWeightClass>(weightClass, true, out var wcEnum))
            {
                query = query.Where(e => (e as ArmwrestlingEvent)!.WeightClass == wcEnum);
            }
        }

        var results = await query
            .OrderByDescending(e => e.Date)
            .ToListAsync();

        // Filter in memory for events with winners (individual or team)
        results = results.Where(e => e.FirstPlaceId != null || 
                                     (e is FootballEvent fe && fe.FirstPlaceTeamId != null) ||
                                     (e is VolleyballEvent ve && ve.FirstPlaceTeamId != null) ||
                                     (e is BasketballEvent be && be.FirstPlaceTeamId != null) ||
                                     (e is BeachVolleyballEvent bve && bve.FirstPlaceTeamId != null))
                         .ToList();

        var dtos = new List<PolytechLeague.Application.Dto.General.ChampionDto>();

        // Add manual entries
        var manualEntriesQuery = _context.HallOfFameManualEntries.AsQueryable();
        if (!string.IsNullOrEmpty(sportType) && sportType != "all")
        {
            manualEntriesQuery = manualEntriesQuery.Where(e => e.Sport == sportType);
        }
        var manualEntries = await manualEntriesQuery.ToListAsync();

        foreach (var me in manualEntries)
        {
            dtos.Add(new PolytechLeague.Application.Dto.General.ChampionDto
            {
                PlayerId = 0,
                PlayerName = me.TeamName,
                PlayerPhotoUrl = me.PhotoUrl,
                SeasonName = me.SeasonName,
                TournamentId = me.Id,
                TournamentName = "Архівні дані",
                SportType = me.Sport,
                Date = me.CreatedAt,
                Points = 0,
                Gender = Gender.None,
                WeightClass = null
            });
        }

        foreach (var e in results)
        {
            double points = 0;
            
            // Try to fetch points from specific participant tables for individual sports
            if (e is ChessEvent)
            {
                points = await _context.ChessEventParticipants
                    .Where(p => p.TournamentId == e.Id && p.PlayerId == e.FirstPlaceId)
                    .Select(p => p.Points)
                    .FirstOrDefaultAsync();
            }
            else if (e is CheckersEvent)
            {
                points = await _context.CheckersEventParticipants
                    .Where(p => p.TournamentId == e.Id && p.PlayerId == e.FirstPlaceId)
                    .Select(p => p.Points)
                    .FirstOrDefaultAsync();
            }
            else if (e is TableTennisEvent)
            {
                points = await _context.TableTennisEventParticipants
                    .Where(p => p.TournamentId == e.Id && p.PlayerId == e.FirstPlaceId)
                    .Select(p => p.Points)
                    .FirstOrDefaultAsync();
            }

            string playerName = "";
            long playerId = 0;
            string? photoUrl = null;
            Gender champGender = Gender.None;

            var teamMembers = new List<PolytechLeague.Application.Dto.General.TeamMemberDto>();

            if (e.FirstPlaceId != null)
            {
                playerName = $"{e.FirstPlace!.FirstName} {e.FirstPlace.LastName}";
                playerId = e.FirstPlaceId.Value;
                photoUrl = e.FirstPlace.PhotoUrl;
                champGender = e.FirstPlace.Gender;
            }
            else if (e is FootballEvent fe && fe.FirstPlaceTeamId != null)
            {
                var team = await _context.FootballTeams.Include(t => t.Players).FirstOrDefaultAsync(t => t.Id == fe.FirstPlaceTeamId);
                playerName = team?.Name ?? "Команда";
                if (team?.Players != null) teamMembers = team.Players.Select(p => new PolytechLeague.Application.Dto.General.TeamMemberDto { Id = p.Id, Name = $"{p.FirstName} {p.LastName}" }).ToList();
            }
            else if (e is VolleyballEvent ve && ve.FirstPlaceTeamId != null)
            {
                var team = await _context.VolleyballTeams.Include(t => t.Players).FirstOrDefaultAsync(t => t.Id == ve.FirstPlaceTeamId);
                playerName = team?.Name ?? "Команда";
                if (team?.Players != null) teamMembers = team.Players.Select(p => new PolytechLeague.Application.Dto.General.TeamMemberDto { Id = p.Id, Name = $"{p.FirstName} {p.LastName}" }).ToList();
            }
            else if (e is BasketballEvent be && be.FirstPlaceTeamId != null)
            {
                var team = await _context.BasketballTeams.Include(t => t.Players).FirstOrDefaultAsync(t => t.Id == be.FirstPlaceTeamId);
                playerName = team?.Name ?? "Команда";
                if (team?.Players != null) teamMembers = team.Players.Select(p => new PolytechLeague.Application.Dto.General.TeamMemberDto { Id = p.Id, Name = $"{p.FirstName} {p.LastName}" }).ToList();
            }
            else if (e is BeachVolleyballEvent bve && bve.FirstPlaceTeamId != null)
            {
                var team = await _context.BeachVolleyballTeams.Include(t => t.Players).FirstOrDefaultAsync(t => t.Id == bve.FirstPlaceTeamId);
                playerName = team?.Name ?? "Команда";
                if (team?.Players != null) teamMembers = team.Players.Select(p => new PolytechLeague.Application.Dto.General.TeamMemberDto { Id = p.Id, Name = $"{p.FirstName} {p.LastName}" }).ToList();
            }

            dtos.Add(new PolytechLeague.Application.Dto.General.ChampionDto
            {
                PlayerId = playerId,
                PlayerName = playerName,
                PlayerPhotoUrl = photoUrl,
                SeasonName = e.Season?.Name ?? e.Date.Year.ToString(),
                TournamentId = e.Id,
                TournamentName = e.Name,
                SportType = GetSportName(e),
                Date = e.Date,
                Points = points,
                Gender = champGender,
                WeightClass = (e is ArmwrestlingEvent armEvent) ? armEvent.WeightClass.ToString() : null,
                TeamMembers = teamMembers
            });
        }

        return dtos;
    }

    public async Task<bool> VoteAsync(long eventId, long userId, long? teamId, long? playerId)
    {
        var e = await _context.Set<Event>().FindAsync(eventId);
        if (e == null) return false;

        if (DateTime.UtcNow >= e.Date) return false;

        var existingVote = await _context.EventVotes
            .FirstOrDefaultAsync(v => v.EventId == eventId && v.UserId == userId);
        if (existingVote != null) return false;

        var vote = new EventVote
        {
            EventId = eventId,
            UserId = userId,
            VotedForTeamId = teamId,
            VotedForPlayerId = playerId,
            VotedAt = DateTime.UtcNow
        };

        _context.EventVotes.Add(vote);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<VoteStatsDto> GetVoteStatsAsync(long eventId, long? userId = null)
    {
        var votes = await _context.EventVotes
            .Where(v => v.EventId == eventId)
            .ToListAsync();

        var totalVotes = votes.Count;
        var stats = new VoteStatsDto { TotalVotes = totalVotes };

        if (userId.HasValue)
        {
            stats.HasVoted = votes.Any(v => v.UserId == userId.Value);
        }

        if (totalVotes == 0) return stats;

        var teamVotes = votes.Where(v => v.VotedForTeamId.HasValue)
            .GroupBy(v => v.VotedForTeamId!.Value)
            .Select(g => new { Id = g.Key, Count = g.Count() })
            .ToList();

        var playerVotes = votes.Where(v => v.VotedForPlayerId.HasValue)
            .GroupBy(v => v.VotedForPlayerId!.Value)
            .Select(g => new { Id = g.Key, Count = g.Count() })
            .ToList();

        var e = await _context.Set<Event>().FindAsync(eventId);
        if (e == null) return stats;

        var sport = GetSportName(e);

        if (teamVotes.Any())
        {
            var teamIds = teamVotes.Select(tv => tv.Id).ToList();
            Dictionary<long, string> teamNames = new();

            switch (sport)
            {
                case "football":
                    teamNames = await _context.FootballTeams.Where(t => teamIds.Contains(t.Id) && t.Name != null).ToDictionaryAsync(t => t.Id, t => t.Name!);
                    break;
                case "volleyball":
                    teamNames = await _context.VolleyballTeams.Where(t => teamIds.Contains(t.Id) && t.Name != null).ToDictionaryAsync(t => t.Id, t => t.Name!);
                    break;
                case "basketball-3x3":
                    teamNames = await _context.BasketballTeams.Where(t => teamIds.Contains(t.Id) && t.Name != null).ToDictionaryAsync(t => t.Id, t => t.Name!);
                    break;
                case "beach-volleyball":
                    teamNames = await _context.BeachVolleyballTeams.Where(t => teamIds.Contains(t.Id) && t.Name != null).ToDictionaryAsync(t => t.Id, t => t.Name!);
                    break;
            }

            foreach (var tv in teamVotes)
            {
                string name = teamNames.ContainsKey(tv.Id) ? teamNames[tv.Id] : "Команда";
                stats.OptionStats.Add(new VoteOptionStatDto
                {
                    Id = tv.Id,
                    Name = name,
                    Votes = tv.Count,
                    Percentage = Math.Round((double)tv.Count / totalVotes * 100, 2)
                });
            }
        }

        if (playerVotes.Any())
        {
            var playerIds = playerVotes.Select(pv => pv.Id).ToList();
            var players = await _context.Players
                .Where(p => playerIds.Contains(p.Id))
                .Select(p => new { p.Id, p.FirstName, p.LastName })
                .ToDictionaryAsync(p => p.Id, p => $"{p.FirstName} {p.LastName}");

            foreach (var pv in playerVotes)
            {
                string name = players.ContainsKey(pv.Id) ? players[pv.Id] : "Гравець";

                stats.OptionStats.Add(new VoteOptionStatDto
                {
                    Id = pv.Id,
                    Name = name,
                    Votes = pv.Count,
                    Percentage = Math.Round((double)pv.Count / totalVotes * 100, 2)
                });
            }
        }

        stats.OptionStats = stats.OptionStats.OrderByDescending(o => o.Votes).ToList();

        return stats;
    }
}
