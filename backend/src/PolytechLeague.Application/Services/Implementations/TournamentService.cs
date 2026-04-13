using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Persistence.Database;
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
        string? sport = null)
    {
        var query = _context.Set<PolytechLeague.Domain.Entities.Event>()
            .Include(e => e.Photos)
            .AsQueryable();

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

        var items = events.Select(e => new TournamentDto
        {
            Id = e.Id,
            Title = e.Name,
            Sport = GetSportName(e),
            Status = GetStatus(e.Date),
            Date = e.Date.ToString("dd.MM.yyyy"),
            Location = e.Location,
            Description = e.Description,
            Participants = 0,
            Teams = 0,
            Prize = "Призи та кубки",
            Organizer = "Спортвідділ",
            RegistrationEnd = e.Date.AddDays(-5).ToString("dd.MM.yyyy"),
            CurrentRound = GetStatus(e.Date) == "ongoing" ? "В процесі" : "Реєстрація",
            Registered = false,
            Featured = false,
            IsTeam = e.IsTeam
        });

        return new PagedResponse<TournamentDto>
        {
            Items = items,
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

        return new TournamentDto
        {
            Id = e.Id,
            Title = e.Name,
            Sport = GetSportName(e),
            Status = GetStatus(e.Date),
            Date = e.Date.ToString("dd.MM.yyyy"),
            Location = e.Location,
            Description = e.Description,
            Participants = 0, // Need to implement counting
            Teams = 0, // Need to implement counting
            Prize = "Призи та кубки",
            Organizer = "Спортвідділ",
            RegistrationEnd = e.Date.AddDays(-5).ToString("dd.MM.yyyy"),
            CurrentRound = GetStatus(e.Date) == "ongoing" ? "В процесі" : "Реєстрація",
            Registered = false,
            Featured = false,
            IsTeam = e.IsTeam
        };
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

    private string GetStatus(DateTime date)
    {
        var now = DateTime.UtcNow;
        if (date > now.AddDays(1)) return "upcoming";
        if (date < now.AddDays(-1)) return "completed";
        return "ongoing";
    }
}
