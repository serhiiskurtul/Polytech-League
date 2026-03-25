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

    public async Task<IEnumerable<TournamentDto>> GetAllTournamentsAsync()
    {
        var events = await _context.Set<PolytechLeague.Domain.Entities.Event>()
            .Include(e => e.Photos)
            .ToListAsync();

        return events.Select(e => new TournamentDto
        {
            Id = e.Id,
            Title = e.Name,
            Sport = GetSportName(e),
            Status = GetStatus(e.Date),
            Date = e.Date.ToString("dd.MM.yyyy"), // Simple format for now
            Location = e.Location,
            Description = e.Description,
            Participants = 0, // Need to implement counting for each type
            Teams = 0, // Need to implement counting for each type
            Prize = "Призи та кубки", // Placeholder
            Organizer = "Спортвідділ", // Placeholder
            RegistrationEnd = e.Date.AddDays(-5).ToString("dd.MM.yyyy"), // Placeholder
            CurrentRound = GetStatus(e.Date) == "ongoing" ? "В процесі" : "Реєстрація",
            Registered = false,
            Featured = false
        });
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
