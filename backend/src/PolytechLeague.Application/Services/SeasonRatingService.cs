using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services;

public class SeasonRatingService : ISeasonRatingService
{
    private readonly ApplicationDbContext _context;
    private const int NON_PARTICIPATION_PENALTY = 25; // Очки за неявку

    public SeasonRatingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SeasonRatingDto> GetSeasonRatingAsync(long seasonId)
    {
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null) throw new Exception("Season not found");

        var sports = new List<string> 
        { 
            "Шахи", "Шашки", "Футзал", "Армреслінг", 
            "Настільний теніс", "Бадмінтон", "Волейбол", 
            "Баскетбол 3x3", "Пляжний волейбол" 
        };

        // Fetch all possible groups
        var playerGroups = await _context.Players
            .Where(p => !string.IsNullOrEmpty(p.Group))
            .Select(p => p.Group)
            .Distinct()
            .ToListAsync();
            
        var futsalGroups = await _context.FutsalTeams.Select(t => t.Name).Distinct().ToListAsync();
        var volleyballGroups = await _context.VolleyballTeams.Select(t => t.Name).Distinct().ToListAsync();
        var basketballGroups = await _context.Basketball3x3Teams.Select(t => t.Name).Distinct().ToListAsync();
        var beachVolleyballGroups = await _context.BeachVolleyballTeams.Select(t => t.Name).Distinct().ToListAsync();
        
        var allGroups = playerGroups
            .Union(futsalGroups)
            .Union(volleyballGroups)
            .Union(basketballGroups)
            .Union(beachVolleyballGroups)
            .Where(g => !string.IsNullOrEmpty(g))
            .Distinct()
            .OrderBy(g => g)
            .ToList();

        var rows = allGroups.Select(g => new GroupRatingRowDto
        {
            GroupName = g,
            SportResults = sports.ToDictionary(s => s, s => new SportResultDto { Points = NON_PARTICIPATION_PENALTY, Rank = 0 }),
            TotalPoints = 0,
            TotalRank = 0
        }).ToList();

        // 1. Chess
        var chessEvents = await _context.ChessEvents
            .Where(e => e.SeasonId == seasonId && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Players)
            .ToListAsync();
        foreach (var e in chessEvents)
        {
            foreach (var p in e.Players ?? new List<Domain.Entities.Chess.ChessEventParticipant>())
            {
                var groupName = p.GroupName ?? p.Player?.Group;
                if (string.IsNullOrEmpty(groupName)) continue;
                
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));
                if (row != null && p.Rank > 0)
                {
                    if (row.SportResults["Шахи"].Rank == 0 || p.Rank < row.SportResults["Шахи"].Rank)
                    {
                        row.SportResults["Шахи"] = new SportResultDto { Rank = p.Rank, Points = p.Rank };
                    }
                }
            }
        }

        // 2. Checkers
        var checkersEvents = await _context.CheckersEvents
            .Where(e => e.SeasonId == seasonId && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Players)
            .ToListAsync();
        foreach (var e in checkersEvents)
        {
            foreach (var p in e.Players ?? new List<Domain.Entities.Checkers.CheckersEventParticipant>())
            {
                var groupName = p.GroupName ?? p.Player?.Group;
                if (string.IsNullOrEmpty(groupName)) continue;
                
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));
                if (row != null && p.Rank > 0)
                {
                    if (row.SportResults["Шашки"].Rank == 0 || p.Rank < row.SportResults["Шашки"].Rank)
                    {
                        row.SportResults["Шашки"] = new SportResultDto { Rank = p.Rank, Points = p.Rank };
                    }
                }
            }
        }

        // 3. Futsal
        var futsalEvents = await _context.FutsalEvents
            .Where(e => e.SeasonId == seasonId && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Teams)
            .ToListAsync();
        foreach (var e in futsalEvents)
        {
            foreach (var t in e.Teams ?? new List<Domain.Entities.Futsal.FutsalTeam>())
            {
                if (string.IsNullOrEmpty(t.Name)) continue;
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(t.Name, StringComparison.OrdinalIgnoreCase));
                if (row != null && t.Rank > 0)
                {
                    if (row.SportResults["Футзал"].Rank == 0 || t.Rank < row.SportResults["Футзал"].Rank)
                    {
                        row.SportResults["Футзал"] = new SportResultDto { Rank = t.Rank, Points = t.Rank };
                    }
                }
            }
        }

        // 4. Armwrestling
        var armEvents = await _context.ArmwrestlingEvents
            .Where(e => e.SeasonId == seasonId && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Players)
            .ToListAsync();
        foreach (var e in armEvents)
        {
            foreach (var p in e.Players ?? new List<Domain.Entities.Armwrestling.ArmwrestlingEventParticipant>())
            {
                var groupName = p.GroupName ?? p.Player?.Group;
                if (string.IsNullOrEmpty(groupName)) continue;
                
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));
                if (row != null && p.Rank > 0)
                {
                    if (row.SportResults["Армреслінг"].Rank == 0 || p.Rank < row.SportResults["Армреслінг"].Rank)
                    {
                        row.SportResults["Армреслінг"] = new SportResultDto { Rank = p.Rank, Points = p.Rank };
                    }
                }
            }
        }

        // 5. Table Tennis
        var ttEvents = await _context.TableTennisEvents
            .Where(e => e.SeasonId == seasonId && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Participants)
            .ToListAsync();
        foreach (var e in ttEvents)
        {
            foreach (var p in e.Participants ?? new List<Domain.Entities.TableTennis.TableTennisEventParticipant>())
            {
                var groupName = p.GroupName ?? p.Player?.Group;
                if (string.IsNullOrEmpty(groupName)) continue;
                
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));
                if (row != null && p.Rank > 0)
                {
                    if (row.SportResults["Настільний теніс"].Rank == 0 || p.Rank < row.SportResults["Настільний теніс"].Rank)
                    {
                        row.SportResults["Настільний теніс"] = new SportResultDto { Rank = p.Rank, Points = p.Rank };
                    }
                }
            }
        }

        // 6. Badminton
        var badEvents = await _context.BadmintonEvents
            .Where(e => e.SeasonId == seasonId && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Participants)
            .ToListAsync();
        foreach (var e in badEvents)
        {
            foreach (var p in e.Participants ?? new List<Domain.Entities.Badminton.BadmintonEventParticipant>())
            {
                var groupName = p.GroupName ?? p.Player?.Group;
                if (string.IsNullOrEmpty(groupName)) continue;
                
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));
                if (row != null && p.Rank > 0)
                {
                    if (row.SportResults["Бадмінтон"].Rank == 0 || p.Rank < row.SportResults["Бадмінтон"].Rank)
                    {
                        row.SportResults["Бадмінтон"] = new SportResultDto { Rank = p.Rank, Points = p.Rank };
                    }
                }
            }
        }

        // 7. Volleyball
        var volEvents = await _context.VolleyballEvents
            .Where(e => e.SeasonId == seasonId && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Teams)
            .ToListAsync();
        foreach (var e in volEvents)
        {
            foreach (var t in e.Teams ?? new List<Domain.Entities.Volleyball.VolleyballTeam>())
            {
                if (string.IsNullOrEmpty(t.Name)) continue;
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(t.Name, StringComparison.OrdinalIgnoreCase));
                if (row != null && t.Rank > 0)
                {
                    if (row.SportResults["Волейбол"].Rank == 0 || t.Rank < row.SportResults["Волейбол"].Rank)
                    {
                        row.SportResults["Волейбол"] = new SportResultDto { Rank = t.Rank, Points = t.Rank };
                    }
                }
            }
        }

        // 8. Basketball 3x3
        var bballEvents = await _context.Basketball3x3Events
            .Where(e => e.SeasonId == seasonId && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Teams)
            .ToListAsync();
        foreach (var e in bballEvents)
        {
            foreach (var t in e.Teams ?? new List<Domain.Entities.Basketball3x3.Basketball3x3Team>())
            {
                if (string.IsNullOrEmpty(t.Name)) continue;
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(t.Name, StringComparison.OrdinalIgnoreCase));
                if (row != null && t.Rank > 0)
                {
                    if (row.SportResults["Баскетбол 3x3"].Rank == 0 || t.Rank < row.SportResults["Баскетбол 3x3"].Rank)
                    {
                        row.SportResults["Баскетбол 3x3"] = new SportResultDto { Rank = t.Rank, Points = t.Rank };
                    }
                }
            }
        }

        // 9. Beach Volleyball
        var bvolEvents = await _context.BeachVolleyballEvents
            .Where(e => e.SeasonId == seasonId && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Teams)
            .ToListAsync();
        foreach (var e in bvolEvents)
        {
            foreach (var t in e.Teams ?? new List<Domain.Entities.BeachVolleyball.BeachVolleyballTeam>())
            {
                if (string.IsNullOrEmpty(t.Name)) continue;
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(t.Name, StringComparison.OrdinalIgnoreCase));
                if (row != null && t.Rank > 0)
                {
                    if (row.SportResults["Пляжний волейбол"].Rank == 0 || t.Rank < row.SportResults["Пляжний волейбол"].Rank)
                    {
                        row.SportResults["Пляжний волейбол"] = new SportResultDto { Rank = t.Rank, Points = t.Rank };
                    }
                }
            }
        }

        // Calculate Totals
        foreach (var row in rows)
        {
            row.TotalPoints = row.SportResults.Values.Sum(v => v.Points);
        }

        // Calculate Total Ranks (Lowest Points = 1st Rank)
        var orderedRows = rows.OrderBy(r => r.TotalPoints).ToList();
        for (int i = 0; i < orderedRows.Count; i++)
        {
            orderedRows[i].TotalRank = i + 1;
        }

        return new SeasonRatingDto
        {
            SeasonId = season.Id,
            SeasonName = season.Name,
            Sports = sports,
            Rows = orderedRows
        };
    }

    public async Task<IEnumerable<SeasonDto>> GetAllSeasonsAsync()
    {
        return await _context.Seasons
            .OrderByDescending(s => s.StartDate)
            .Select(s => new SeasonDto
            {
                Id = s.Id,
                Name = s.Name,
                IsCurrent = s.IsCurrent,
                StartDate = s.StartDate,
                EndDate = s.EndDate
            }).ToListAsync();
    }

    public async Task<SeasonDto?> GetCurrentSeasonAsync()
    {
        var season = await _context.Seasons.FirstOrDefaultAsync(s => s.IsCurrent);
        if (season == null) return null;
        return new SeasonDto
        {
            Id = season.Id,
            Name = season.Name,
            IsCurrent = season.IsCurrent,
            StartDate = season.StartDate,
            EndDate = season.EndDate
        };
    }

    public async Task<SeasonDto> CreateSeasonAsync(SeasonDto dto)
    {
        if (dto.IsCurrent)
        {
            var current = await _context.Seasons.FirstOrDefaultAsync(s => s.IsCurrent);
            if (current != null) current.IsCurrent = false;
        }

        var season = new Season
        {
            Name = dto.Name,
            IsCurrent = dto.IsCurrent,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
        _context.Seasons.Add(season);
        await _context.SaveChangesAsync();
        dto.Id = season.Id;
        return dto;
    }

    public async Task<SeasonDto> UpdateSeasonAsync(long id, SeasonDto dto)
    {
        var season = await _context.Seasons.FindAsync(id);
        if (season == null) throw new Exception("Season not found");

        if (dto.IsCurrent && !season.IsCurrent)
        {
            var current = await _context.Seasons.FirstOrDefaultAsync(s => s.IsCurrent);
            if (current != null) current.IsCurrent = false;
        }

        season.Name = dto.Name;
        season.IsCurrent = dto.IsCurrent;
        season.StartDate = dto.StartDate;
        season.EndDate = dto.EndDate;
        
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task DeleteSeasonAsync(long id)
    {
        var season = await _context.Seasons.FindAsync(id);
        if (season != null)
        {
            _context.Seasons.Remove(season);
            await _context.SaveChangesAsync();
        }
    }
}
