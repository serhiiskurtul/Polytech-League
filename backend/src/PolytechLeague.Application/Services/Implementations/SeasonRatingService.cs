using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;
using PolytechLeague.Domain.Entities.Chess;
using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Domain.Entities.TableTennis;
using PolytechLeague.Domain.Entities.Armwrestling;
using PolytechLeague.Domain.Entities.Badminton;
using PolytechLeague.Domain.Entities.Football;
using PolytechLeague.Domain.Entities.Volleyball;
using PolytechLeague.Domain.Entities.Basketball;
using PolytechLeague.Domain.Entities.BeachVolleyball;
using PolytechLeague.Domain.Entities.Athletics;

namespace PolytechLeague.Application.Services.Implementations;

public class SeasonRatingService : ISeasonRatingService
{
    private readonly ApplicationDbContext _context;

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
            "Шахи", "Шашки", "Футбол", "Армрестлінг", 
            "Настільний теніс", "Бадмінтон", "Волейбол", 
            "Баскетбол", "Пляжний волейбол", "Легка атлетика" 
        };

        // Fetch all possible groups
        var playerGroups = await _context.Players
            .Where(p => !string.IsNullOrEmpty(p.Group))
            .Select(p => p.Group)
            .Distinct()
            .ToListAsync();
            
        var FootballGroups = await _context.FootballTeams.Select(t => t.Name).Distinct().ToListAsync();
        var volleyballGroups = await _context.VolleyballTeams.Select(t => t.Name).Distinct().ToListAsync();
        var basketballGroups = await _context.BasketballTeams.Select(t => t.Name).Distinct().ToListAsync();
        var beachVolleyballGroups = await _context.BeachVolleyballTeams.Select(t => t.Name).Distinct().ToListAsync();
        var chessGroups = await _context.ChessEventParticipants.Select(p => p.GroupName).Distinct().ToListAsync();
        var checkersGroups = await _context.CheckersEventParticipants.Select(p => p.GroupName).Distinct().ToListAsync();
        var armGroups = await _context.ArmwrestlingEventParticipants.Select(p => p.GroupName).Distinct().ToListAsync();
        var ttGroups = await _context.TableTennisEventParticipants.Select(p => p.GroupName).Distinct().ToListAsync();
        var badGroups = await _context.BadmintonEventParticipants.Select(p => p.GroupName).Distinct().ToListAsync();
        var athleticsGroups = await _context.AthleticsEventParticipants.Select(p => p.GroupName).Distinct().ToListAsync();

        var allGroups = playerGroups
            .Union(FootballGroups)
            .Union(volleyballGroups)
            .Union(basketballGroups)
            .Union(beachVolleyballGroups)
            .Union(chessGroups)
            .Union(checkersGroups)
            .Union(armGroups)
            .Union(ttGroups)
            .Union(badGroups)
            .Union(athleticsGroups)
            .Where(g => !string.IsNullOrEmpty(g) && g.Length >= 3 && char.IsDigit(g[0]) && char.IsDigit(g[1]) && char.IsDigit(g[2]))
            .Select(g => g.Substring(0, 3)) // Normalize to first 3 digits
            .Distinct()
            .OrderBy(g => g)
            .ToList();

        var rows = allGroups.Select(g => new GroupRatingRowDto
        {
            GroupName = g,
            SportResults = sports.ToDictionary(s => s, s => new SportResultDto { Points = 0, Rank = "-" }),
            TotalPoints = 0,
            TotalRank = "-"
        }).ToList();

        // 1. Chess
        var chessEvents = await _context.ChessEvents
            .AsNoTracking()
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Players!)
                .ThenInclude(p => p.Player)
            .Include(e => e.Teams)
            .ToListAsync();
            
        foreach (var e in chessEvents)
        {
            if (e.IsTeam && e.Teams != null && e.Teams.Any())
            {
                foreach (var t in e.Teams)
                {
                    if (string.IsNullOrEmpty(t.Name)) continue;
                    var normalizedTGroup = t.Name.Length >= 3 ? t.Name.Substring(0, 3) : t.Name;
                    var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedTGroup, StringComparison.OrdinalIgnoreCase));
                    if (row != null)
                    {
                        int rank = t.Rank;
                        if (rank <= 0)
                        {
                            if (t.Id == e.FirstPlaceTeamId) rank = 1;
                            else if (t.Id == e.SecondPlaceTeamId) rank = 2;
                            else if (t.Id == e.ThirdPlaceTeamId) rank = 3;
                            else if (e.FirstPlaceTeamId != null) rank = 4;
                            else rank = 4;
                        }
                        
                        if (row.SportResults["Шахи"].Rank == "-" || rank < int.Parse(row.SportResults["Шахи"].Rank!))
                        {
                            row.SportResults["Шахи"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                        }
                    }
                }
            }
            else
            {
                foreach (var p in e.Players ?? new List<Domain.Entities.Chess.ChessEventParticipant>())
                {
                    var groupName = p.GroupName ?? p.Player?.Group;
                    if (string.IsNullOrEmpty(groupName)) continue;
                    var normalizedPGroup = groupName.Length >= 3 ? groupName.Substring(0, 3) : groupName;
                    
                    var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedPGroup, StringComparison.OrdinalIgnoreCase));
                    if (row != null)
                    {
                        int rank = p.Rank;
                        if (rank <= 0)
                        {
                            if (e.FirstPlaceId != null && p.PlayerId == e.FirstPlaceId) rank = 1;
                            else if (e.SecondPlaceId != null && p.PlayerId == e.SecondPlaceId) rank = 2;
                            else if (e.ThirdPlaceId != null && p.PlayerId == e.ThirdPlaceId) rank = 3;
                            else if (e.FirstPlaceId != null) rank = 4;
                            else rank = 4;
                        }
                        
                        if (row.SportResults["Шахи"].Rank == "-" || rank < int.Parse(row.SportResults["Шахи"].Rank!))
                        {
                            row.SportResults["Шахи"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                        }
                    }
                }
            }
        }

        // 2. Checkers
        var checkersEvents = await _context.CheckersEvents
            .AsNoTracking()
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Players!)
                .ThenInclude(p => p.Player)
            .Include(e => e.Teams)
            .ToListAsync();
            
        foreach (var e in checkersEvents)
        {
            if (e.IsTeam && e.Teams != null && e.Teams.Any())
            {
                foreach (var t in e.Teams)
                {
                    if (string.IsNullOrEmpty(t.Name)) continue;
                    var normalizedTGroup = t.Name.Length >= 3 ? t.Name.Substring(0, 3) : t.Name;
                    var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedTGroup, StringComparison.OrdinalIgnoreCase));
                    if (row != null)
                    {
                        int rank = t.Rank;
                        if (rank <= 0)
                        {
                            if (t.Id == e.FirstPlaceTeamId) rank = 1;
                            else if (t.Id == e.SecondPlaceTeamId) rank = 2;
                            else if (t.Id == e.ThirdPlaceTeamId) rank = 3;
                            else if (e.FirstPlaceTeamId != null) rank = 4;
                            else rank = 4;
                        }
                        
                        if (row.SportResults["Шашки"].Rank == "-" || rank < int.Parse(row.SportResults["Шашки"].Rank!))
                        {
                            row.SportResults["Шашки"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                        }
                    }
                }
            }
            else
            {
                foreach (var p in e.Players ?? new List<Domain.Entities.Checkers.CheckersEventParticipant>())
                {
                    var groupName = p.GroupName ?? p.Player?.Group;
                    if (string.IsNullOrEmpty(groupName)) continue;
                    var normalizedPGroup = groupName.Length >= 3 ? groupName.Substring(0, 3) : groupName;
                    
                    var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedPGroup, StringComparison.OrdinalIgnoreCase));
                    if (row != null)
                    {
                        int rank = p.Rank;
                        if (rank <= 0)
                        {
                            if (e.FirstPlaceId != null && p.PlayerId == e.FirstPlaceId) rank = 1;
                            else if (e.SecondPlaceId != null && p.PlayerId == e.SecondPlaceId) rank = 2;
                            else if (e.ThirdPlaceId != null && p.PlayerId == e.ThirdPlaceId) rank = 3;
                            else if (e.FirstPlaceId != null) rank = 4;
                            else rank = 4;
                        }
                        
                        if (row.SportResults["Шашки"].Rank == "-" || rank < int.Parse(row.SportResults["Шашки"].Rank!))
                        {
                            row.SportResults["Шашки"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                        }
                    }
                }
            }
        }

        // 3. Football
        var FootballEvents = await _context.FootballEvents
            .AsNoTracking()
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Teams)
            .ToListAsync();
        foreach (var e in FootballEvents)
        {
            foreach (var t in e.Teams ?? new List<Domain.Entities.Football.FootballTeam>())
            {
                if (string.IsNullOrEmpty(t.Name)) continue;
                var normalizedTGroup = t.Name.Length >= 3 ? t.Name.Substring(0, 3) : t.Name;
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedTGroup, StringComparison.OrdinalIgnoreCase));
                if (row != null)
                {
                    int rank = t.Rank;
                    if (rank <= 0)
                    {
                        if (t.Id == e.FirstPlaceTeamId) rank = 1;
                        else if (t.Id == e.SecondPlaceTeamId) rank = 2;
                        else if (t.Id == e.ThirdPlaceTeamId) rank = 3;
                        else if (e.FirstPlaceTeamId != null) rank = 4;
                        else rank = 4;
                    }
                    
                    if (row.SportResults["Футбол"].Rank == "-" || rank < int.Parse(row.SportResults["Футбол"].Rank!))
                    {
                        row.SportResults["Футбол"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                    }
                }
            }
        }

        // 4. Armwrestling
        var armEvents = await _context.ArmwrestlingEvents
            .AsNoTracking()
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Players!)
                .ThenInclude(p => p.Player)
            .ToListAsync();

        if (armEvents.Any())
        {
            var groupArmSums = rows.ToDictionary(r => r.GroupName, r => 0);
            var groupHasParticipated = rows.ToDictionary(r => r.GroupName, r => false);

            foreach (var e in armEvents)
            {
                var participants = e.Players ?? new List<Domain.Entities.Armwrestling.ArmwrestlingEventParticipant>();
                
                int penalty = participants.Count + 2;

                var eventGroupResults = participants
                    .Where(p => !string.IsNullOrEmpty(p.GroupName ?? p.Player?.Group))
                    .Select(p => {
                        var g = p.GroupName ?? p.Player?.Group;
                        var digits = new string(g!.Where(char.IsDigit).ToArray());
                        
                        int rank = p.Rank;
                        if (rank <= 0)
                        {
                            if (e.FirstPlaceId != null && p.PlayerId == e.FirstPlaceId) rank = 1;
                            else if (e.SecondPlaceId != null && p.PlayerId == e.SecondPlaceId) rank = 2;
                            else if (e.ThirdPlaceId != null && p.PlayerId == e.ThirdPlaceId) rank = 3;
                            else rank = penalty;
                        }

                        return new { 
                            Group = digits, 
                            Rank = rank 
                        };
                    })
                    .Where(x => !string.IsNullOrEmpty(x.Group))
                    .GroupBy(x => x.Group)
                    .ToDictionary(g => g.Key, g => g.Min(x => x.Rank));

                foreach (var row in rows)
                {
                    var rowDigits = new string(row.GroupName.Where(char.IsDigit).ToArray());
                    if (eventGroupResults.TryGetValue(rowDigits, out int rank))
                    {
                        groupArmSums[row.GroupName] += rank;
                        groupHasParticipated[row.GroupName] = true;
                    }
                    else
                    {
                        groupArmSums[row.GroupName] += penalty;
                    }
                }
            }

            var armOrderedRows = rows
                .OrderBy(r => groupArmSums[r.GroupName])
                .ToList();

            int idx = 0;
            while (idx < armOrderedRows.Count)
            {
                int tieCount = 1;
                while (idx + tieCount < armOrderedRows.Count && groupArmSums[armOrderedRows[idx + tieCount].GroupName] == groupArmSums[armOrderedRows[idx].GroupName])
                {
                    tieCount++;
                }

                string rankStr = tieCount > 1 
                    ? $"{idx + 1}-{idx + tieCount}" 
                    : (idx + 1).ToString();

                for (int j = 0; j < tieCount; j++)
                {
                    var row = armOrderedRows[idx + j];
                    row.SportResults["Армрестлінг"] = new SportResultDto 
                    { 
                        Rank = rankStr, 
                        Points = groupArmSums[row.GroupName] 
                    };
                }
                idx += tieCount;
            }
        }

        // 5. Table Tennis
        var ttEvents = await _context.TableTennisEvents
            .AsNoTracking()
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Participants!)
                .ThenInclude(p => p.Player)
            .Include(e => e.Teams)
            .ToListAsync();
            
        foreach (var e in ttEvents)
        {
            if (e.IsTeam && e.Teams != null && e.Teams.Any())
            {
                foreach (var t in e.Teams)
                {
                    if (string.IsNullOrEmpty(t.Name)) continue;
                    var normalizedTGroup = t.Name.Length >= 3 ? t.Name.Substring(0, 3) : t.Name;
                    var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedTGroup, StringComparison.OrdinalIgnoreCase));
                    if (row != null)
                    {
                        int rank = t.Rank;
                        if (rank <= 0)
                        {
                            if (t.Id == e.FirstPlaceTeamId) rank = 1;
                            else if (t.Id == e.SecondPlaceTeamId) rank = 2;
                            else if (t.Id == e.ThirdPlaceTeamId) rank = 3;
                            else if (e.FirstPlaceTeamId != null) rank = 4;
                            else rank = 4;
                        }
                        
                        if (row.SportResults["Настільний теніс"].Rank == "-" || rank < int.Parse(row.SportResults["Настільний теніс"].Rank!))
                        {
                            row.SportResults["Настільний теніс"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                        }
                    }
                }
            }
            else
            {
                foreach (var p in e.Participants ?? new List<Domain.Entities.TableTennis.TableTennisEventParticipant>())
                {
                    var groupName = p.GroupName ?? p.Player?.Group;
                    if (string.IsNullOrEmpty(groupName)) continue;
                    var normalizedPGroup = groupName.Length >= 3 ? groupName.Substring(0, 3) : groupName;
                    
                    var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedPGroup, StringComparison.OrdinalIgnoreCase));
                    if (row != null)
                    {
                        int rank = p.Rank;
                        if (rank <= 0)
                        {
                            if (e.FirstPlaceId != null && p.PlayerId == e.FirstPlaceId) rank = 1;
                            else if (e.SecondPlaceId != null && p.PlayerId == e.SecondPlaceId) rank = 2;
                            else if (e.ThirdPlaceId != null && p.PlayerId == e.ThirdPlaceId) rank = 3;
                            else if (e.FirstPlaceId != null) rank = 4;
                            else rank = 4;
                        }
                        
                        if (row.SportResults["Настільний теніс"].Rank == "-" || rank < int.Parse(row.SportResults["Настільний теніс"].Rank!))
                        {
                            row.SportResults["Настільний теніс"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                        }
                    }
                }
            }
        }

        // 6. Badminton
        var badEvents = await _context.BadmintonEvents
            .AsNoTracking()
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Participants!)
                .ThenInclude(p => p.Player)
            .Include(e => e.Teams)
            .ToListAsync();
            
        foreach (var e in badEvents)
        {
            if (e.IsTeam && e.Teams != null && e.Teams.Any())
            {
                foreach (var t in e.Teams)
                {
                    if (string.IsNullOrEmpty(t.Name)) continue;
                    var normalizedTGroup = t.Name.Length >= 3 ? t.Name.Substring(0, 3) : t.Name;
                    var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedTGroup, StringComparison.OrdinalIgnoreCase));
                    if (row != null)
                    {
                        int rank = t.Rank;
                        if (rank <= 0)
                        {
                            if (t.Id == e.FirstPlaceTeamId) rank = 1;
                            else if (t.Id == e.SecondPlaceTeamId) rank = 2;
                            else if (t.Id == e.ThirdPlaceTeamId) rank = 3;
                            else if (e.FirstPlaceTeamId != null) rank = 4;
                            else rank = 4;
                        }
                        
                        if (row.SportResults["Бадмінтон"].Rank == "-" || rank < int.Parse(row.SportResults["Бадмінтон"].Rank!))
                        {
                            row.SportResults["Бадмінтон"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                        }
                    }
                }
            }
            else
            {
                foreach (var p in e.Participants ?? new List<Domain.Entities.Badminton.BadmintonEventParticipant>())
                {
                    var groupName = p.GroupName ?? p.Player?.Group;
                    if (string.IsNullOrEmpty(groupName)) continue;
                    var normalizedPGroup = groupName.Length >= 3 ? groupName.Substring(0, 3) : groupName;
                    
                    var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedPGroup, StringComparison.OrdinalIgnoreCase));
                    if (row != null)
                    {
                        int rank = p.Rank;
                        if (rank <= 0)
                        {
                            if (e.FirstPlaceId != null && p.PlayerId == e.FirstPlaceId) rank = 1;
                            else if (e.SecondPlaceId != null && p.PlayerId == e.SecondPlaceId) rank = 2;
                            else if (e.ThirdPlaceId != null && p.PlayerId == e.ThirdPlaceId) rank = 3;
                            else if (e.FirstPlaceId != null) rank = 4;
                            else rank = 4;
                        }
                        
                        if (row.SportResults["Бадмінтон"].Rank == "-" || rank < int.Parse(row.SportResults["Бадмінтон"].Rank!))
                        {
                            row.SportResults["Бадмінтон"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                        }
                    }
                }
            }
        }

        // 7. Volleyball
        var volEvents = await _context.VolleyballEvents
            .AsNoTracking()
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Teams)
            .ToListAsync();
        foreach (var e in volEvents)
        {
            foreach (var t in e.Teams ?? new List<Domain.Entities.Volleyball.VolleyballTeam>())
            {
                if (string.IsNullOrEmpty(t.Name)) continue;
                var normalizedTGroup = t.Name.Length >= 3 ? t.Name.Substring(0, 3) : t.Name;
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedTGroup, StringComparison.OrdinalIgnoreCase));
                if (row != null)
                {
                    int rank = t.Rank;
                    if (rank <= 0)
                    {
                        if (t.Id == e.FirstPlaceTeamId) rank = 1;
                        else if (t.Id == e.SecondPlaceTeamId) rank = 2;
                        else if (t.Id == e.ThirdPlaceTeamId) rank = 3;
                        else if (e.FirstPlaceTeamId != null) rank = 4;
                        else rank = 4;
                    }
                    
                    if (row.SportResults["Волейбол"].Rank == "-" || rank < int.Parse(row.SportResults["Волейбол"].Rank!))
                    {
                        row.SportResults["Волейбол"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                    }
                }
            }
        }

        // 8. Basketball 3x3
        var bballEvents = await _context.BasketballEvents
            .AsNoTracking()
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Teams)
            .ToListAsync();
        foreach (var e in bballEvents)
        {
            foreach (var t in e.Teams ?? new List<Domain.Entities.Basketball.BasketballTeam>())
            {
                if (string.IsNullOrEmpty(t.Name)) continue;
                var normalizedTGroup = t.Name.Length >= 3 ? t.Name.Substring(0, 3) : t.Name;
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedTGroup, StringComparison.OrdinalIgnoreCase));
                if (row != null)
                {
                    int rank = t.Rank;
                    if (rank <= 0)
                    {
                        if (t.Id == e.FirstPlaceTeamId) rank = 1;
                        else if (t.Id == e.SecondPlaceTeamId) rank = 2;
                        else if (t.Id == e.ThirdPlaceTeamId) rank = 3;
                        else if (e.FirstPlaceTeamId != null) rank = 4;
                        else rank = 4;
                    }
                    
                    if (row.SportResults["Баскетбол"].Rank == "-" || rank < int.Parse(row.SportResults["Баскетбол"].Rank!))
                    {
                        row.SportResults["Баскетбол"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                    }
                }
            }
        }

        // 9. Beach Volleyball
        var bvolEvents = await _context.BeachVolleyballEvents
            .AsNoTracking()
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Teams)
            .ToListAsync();
        foreach (var e in bvolEvents)
        {
            foreach (var t in e.Teams ?? new List<Domain.Entities.BeachVolleyball.BeachVolleyballTeam>())
            {
                if (string.IsNullOrEmpty(t.Name)) continue;
                var normalizedTGroup = t.Name.Length >= 3 ? t.Name.Substring(0, 3) : t.Name;
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedTGroup, StringComparison.OrdinalIgnoreCase));
                if (row != null)
                {
                    int rank = t.Rank;
                    if (rank <= 0)
                    {
                        if (t.Id == e.FirstPlaceTeamId) rank = 1;
                        else if (t.Id == e.SecondPlaceTeamId) rank = 2;
                        else if (t.Id == e.ThirdPlaceTeamId) rank = 3;
                        else if (e.FirstPlaceTeamId != null) rank = 4;
                        else rank = 4;
                    }
                    
                    if (row.SportResults["Пляжний волейбол"].Rank == "-" || rank < int.Parse(row.SportResults["Пляжний волейбол"].Rank!))
                    {
                        row.SportResults["Пляжний волейбол"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                    }
                }
            }
        }

        // Calculate Totals and use "sport participants + 2" penalty per sport for those who didn't participate
        
        // 10. Athletics
        var athleticsEvents = await _context.AthleticsEvents
            .AsNoTracking()
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Results)
            .Include(e => e.Participants)
                .ThenInclude(p => p.Player)
            .ToListAsync();

        if (athleticsEvents.Any())
        {
            var groupAthleticsPoints = rows.ToDictionary(r => r.GroupName, r => 0);
            var groupHasParticipated = rows.ToDictionary(r => r.GroupName, r => false);

            foreach (var e in athleticsEvents)
            {
                var eventGroupPoints = new Dictionary<string, int>();

                foreach (var r in e.Results)
                {
                    string? team = r.TeamName;
                    if (string.IsNullOrEmpty(team) && r.PlayerId.HasValue)
                    {
                        var participant = e.Participants.FirstOrDefault(p => p.PlayerId == r.PlayerId.Value);
                        if (participant != null)
                        {
                            team = participant.GroupName ?? participant.Player?.Group;
                        }
                    }

                    if (!string.IsNullOrEmpty(team))
                    {
                        var digits = new string(team.Where(char.IsDigit).ToArray());
                        if (digits.Length >= 3) digits = digits.Substring(0, 3);
                        if (!string.IsNullOrEmpty(digits))
                        {
                            int points = r.Rank switch
                            {
                                1 => 10,
                                2 => 8,
                                3 => 6,
                                4 => 5,
                                5 => 4,
                                6 => 3,
                                7 => 2,
                                8 => 1,
                                _ => 0
                            };
                            
                            if (eventGroupPoints.ContainsKey(digits))
                                eventGroupPoints[digits] += points;
                            else
                                eventGroupPoints[digits] = points;
                        }
                    }
                }

                foreach (var row in rows)
                {
                    var rowDigits = new string(row.GroupName.Where(char.IsDigit).ToArray());
                    if (rowDigits.Length >= 3) rowDigits = rowDigits.Substring(0, 3);
                    if (eventGroupPoints.TryGetValue(rowDigits, out int points))
                    {
                        groupAthleticsPoints[row.GroupName] += points;
                        groupHasParticipated[row.GroupName] = true;
                    }
                }
            }

            var participatingRows = rows
                .Where(r => groupHasParticipated[r.GroupName])
                .OrderByDescending(r => groupAthleticsPoints[r.GroupName])
                .ToList();

            int idx = 0;
            while (idx < participatingRows.Count)
            {
                int tieCount = 1;
                while (idx + tieCount < participatingRows.Count && groupAthleticsPoints[participatingRows[idx + tieCount].GroupName] == groupAthleticsPoints[participatingRows[idx].GroupName])
                {
                    tieCount++;
                }

                string rankStr = tieCount > 1 
                    ? $"{idx + 1}-{idx + tieCount}" 
                    : (idx + 1).ToString();

                for (int j = 0; j < tieCount; j++)
                {
                    var row = participatingRows[idx + j];
                    row.SportResults["Легка атлетика"] = new SportResultDto 
                    { 
                        Rank = rankStr, 
                        Points = idx + 1
                    };
                }
                idx += tieCount;
            }
        }

        foreach (var sport in sports)
        {
            // Count how many teams actually participated in this sport
            int participantsCount = rows.Count(r => r.SportResults[sport].Rank != "-");
            
            if (participantsCount > 0)
            {
                int penalty = participantsCount + 2;
                foreach (var row in rows)
                {
                    if (row.SportResults[sport].Rank == "-")
                    {
                        row.SportResults[sport].Points = penalty;
                    }
                }
            }
        }

        foreach (var row in rows)
        {
            row.TotalPoints = row.SportResults.Values.Sum(v => v.Points);
        }

        // Calculate Total Ranks with shared places (e.g. "5-6")
        var orderedRows = rows.OrderBy(r => r.TotalPoints).ToList();
        int i = 0;
        while (i < orderedRows.Count)
        {
            int tieCount = 1;
            while (i + tieCount < orderedRows.Count && orderedRows[i + tieCount].TotalPoints == orderedRows[i].TotalPoints)
            {
                tieCount++;
            }

            string rankStr = tieCount > 1 
                ? $"{i + 1}-{i + tieCount}" 
                : (i + 1).ToString();

            for (int j = 0; j < tieCount; j++)
            {
                orderedRows[i + j].TotalRank = rankStr;
            }
            i += tieCount;
        }

        return new SeasonRatingDto
        {
            SeasonId = season.Id,
            SeasonName = season.Name,
            SpartakiadNumber = season.SpartakiadNumber,
            Sports = sports,
            Rows = orderedRows
        };
    }

    public async Task<SeasonRatingDto> GetNationalRatingAsync(long seasonId)
    {
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null) throw new Exception("Season not found");

        var allSports = new List<string> 
        { 
            "Шахи", "Шашки", "Футбол", "Армрестлінг", 
            "Настільний теніс", "Бадмінтон", "Волейбол (Дівчата)", "Волейбол (Юнаки)", 
            "Баскетбол", "Пляжний волейбол", "Легка атлетика"
        };

        var sports = allSports;
        if (!string.IsNullOrEmpty(season.NationalRatingSports))
        {
            try {
                var loadedSports = System.Text.Json.JsonSerializer.Deserialize<List<string>>(season.NationalRatingSports);
                if (loadedSports != null)
                {
                    sports = loadedSports
                        .Select(s => s == "Армреслінг" ? "Армрестлінг" : s)
                        .Select(s => s == "Футзал" ? "Футбол" : s)
                        .Select(s => (s == "Баскетбол 3x3" || s == "Баскетбол 3х3") ? "Баскетбол" : s)
                        .Select(s => s == "Волейбол" ? "Волейбол (Юнаки)" : s)
                        .Where(s => allSports.Contains(s))
                        .Distinct()
                        .ToList();
                }
            } catch { /* Use all sports if JSON is invalid */ }
        }

        var colleges = await _context.Colleges.AsNoTracking().ToListAsync();
        var manualEntries = await _context.NationalRatingManualEntries
            .AsNoTracking()
            .Where(e => e.SeasonId == seasonId)
            .ToListAsync();

        var nationalEvents = await _context.Set<Event>()
            .AsNoTracking()
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) && e.Level == CompetitionLevel.National)
            .ToListAsync();

        var sportToEventId = new Dictionary<string, long>();
        foreach (var e in nationalEvents)
        {
            string sportKey = e switch
            {
                ChessEvent => "Шахи",
                CheckersEvent => "Шашки",
                TableTennisEvent => "Настільний теніс",
                ArmwrestlingEvent => "Армрестлінг",
                BadmintonEvent => "Бадмінтон",
                FootballEvent => "Футбол",
                VolleyballEvent => e.Category == EventCategory.Women ? "Волейбол (Дівчата)" : "Волейбол (Юнаки)",
                BasketballEvent => "Баскетбол",
                BeachVolleyballEvent => "Пляжний волейбол",
                AthleticsEvent => "Легка атлетика",
                _ => ""
            };
            if (!string.IsNullOrEmpty(sportKey) && !sportToEventId.ContainsKey(sportKey))
            {
                sportToEventId[sportKey] = e.Id;
            }
        }

        var rows = colleges
            .Where(c => manualEntries.Any(e => e.CollegeId == c.Id))
            .Select(c => {
                var entry = manualEntries.First(e => e.CollegeId == c.Id);
                var sportResults = new Dictionary<string, SportResultDto>();
                
                if (!string.IsNullOrEmpty(entry.SportResults))
                {
                    try 
                    {
                        var manualSportResults = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(entry.SportResults);
                        if (manualSportResults != null)
                        {
                            foreach (var kvp in manualSportResults)
                            {
                                //map legacy keys to correct spelling for standings
                                string key = kvp.Key == "Армреслінг" ? "Армрестлінг" : kvp.Key;
                                key = key == "Футзал" ? "Футбол" : key;
                                key = (key == "Баскетбол 3x3" || key == "Баскетбол 3х3") ? "Баскетбол" : key;
                                key = key == "Волейбол" ? "Волейбол (Юнаки)" : key;
                                if (sports.Contains(key))
                                {
                                    long? tId = null;
                                    if (sportToEventId.TryGetValue(key, out var id)) tId = id;
                                    sportResults[key] = new SportResultDto { Rank = kvp.Value, Points = 0, TournamentId = tId };
                                }
                            }
                        }
                    }
                    catch { /* Ignore invalid JSON */ }
                }

                // Fill missing sports with "-"
                foreach (var s in sports)
                {
                    if (!sportResults.ContainsKey(s))
                    {
                        long? tId = null;
                        if (sportToEventId.TryGetValue(s, out var id)) tId = id;
                        sportResults[s] = new SportResultDto { Rank = "-", Points = 0, TournamentId = tId };
                    }
                }

                return new GroupRatingRowDto
                {
                    CollegeId = c.Id,
                    GroupName = c.ShortName ?? c.Name,
                    SportResults = sportResults,
                    TotalPoints = entry.Points,
                    TotalRank = !string.IsNullOrWhiteSpace(entry.Rank) ? entry.Rank : "-"
                };
            }).OrderBy(r => {
                if (r.TotalRank == "-") return int.MaxValue;
                var parts = r.TotalRank!.Split('-');
                if (int.TryParse(parts[0], out int rank)) return rank;
                return int.MaxValue;
            }).ToList();

        return new SeasonRatingDto
        {
            SeasonId = season.Id,
            SeasonName = season.Name,
            Sports = sports,
            Rows = rows
        };
    }

    public async Task UpdateNationalRatingAsync(long seasonId, UpdateManualRatingRequestDto request)
    {
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null) throw new Exception("Season not found");

        if (request.ActiveSports != null)
        {
            season.NationalRatingSports = System.Text.Json.JsonSerializer.Serialize(request.ActiveSports);
        }

        var existingEntries = await _context.NationalRatingManualEntries
            .Where(e => e.SeasonId == seasonId)
            .ToListAsync();

        _context.NationalRatingManualEntries.RemoveRange(existingEntries);

        foreach (var entry in request.Entries)
        {
            string? sportResultsJson = null;
            if (entry.SportResults != null && entry.SportResults.Count > 0)
            {
                sportResultsJson = System.Text.Json.JsonSerializer.Serialize(entry.SportResults);
            }

            _context.NationalRatingManualEntries.Add(new NationalRatingManualEntry
            {
                SeasonId = seasonId,
                CollegeId = entry.CollegeId,
                Points = entry.Points,
                Rank = entry.Rank,
                SportResults = sportResultsJson
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<SeasonRatingDto> GetRegionalRatingAsync(long seasonId)
    {
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null) throw new Exception("Season not found");

        var allSports = new List<string> 
        { 
            "Шахи", "Шашки", "Футбол", "Армрестлінг", 
            "Настільний теніс", "Бадмінтон", "Волейбол (Дівчата)", "Волейбол (Юнаки)", 
            "Баскетбол", "Пляжний волейбол", "Легка атлетика"
        };

        var sports = allSports;
        if (!string.IsNullOrEmpty(season.RegionalRatingSports))
        {
            try {
                var loadedSports = System.Text.Json.JsonSerializer.Deserialize<List<string>>(season.RegionalRatingSports);
                if (loadedSports != null)
                {
                    sports = loadedSports
                        .Select(s => s == "Армреслінг" ? "Армрестлінг" : s)
                        .Select(s => s == "Футзал" ? "Футбол" : s)
                        .Select(s => (s == "Баскетбол 3x3" || s == "Баскетбол 3х3") ? "Баскетбол" : s)
                        .Select(s => s == "Волейбол" ? "Волейбол (Юнаки)" : s)
                        .Where(s => allSports.Contains(s))
                        .Distinct()
                        .ToList();
                }
            } catch { /* Use all sports if JSON is invalid */ }
        }

        var colleges = await _context.Colleges.AsNoTracking().ToListAsync();
        var manualEntries = await _context.RegionalRatingManualEntries
            .AsNoTracking()
            .Where(e => e.SeasonId == seasonId)
            .ToListAsync();

        var regionalEvents = await _context.Set<Event>()
            .AsNoTracking()
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) && e.Level == CompetitionLevel.Regional)
            .ToListAsync();

        var sportToEventId = new Dictionary<string, long>();
        foreach (var e in regionalEvents)
        {
            string sportKey = e switch
            {
                ChessEvent => "Шахи",
                CheckersEvent => "Шашки",
                TableTennisEvent => "Настільний теніс",
                ArmwrestlingEvent => "Армрестлінг",
                BadmintonEvent => "Бадмінтон",
                FootballEvent => "Футбол",
                VolleyballEvent => e.Category == EventCategory.Women ? "Волейбол (Дівчата)" : "Волейбол (Юнаки)",
                BasketballEvent => "Баскетбол",
                BeachVolleyballEvent => "Пляжний волейбол",
                AthleticsEvent => "Легка атлетика",
                _ => ""
            };
            if (!string.IsNullOrEmpty(sportKey) && !sportToEventId.ContainsKey(sportKey))
            {
                sportToEventId[sportKey] = e.Id;
            }
        }

        var rows = colleges
            .Where(c => manualEntries.Any(e => e.CollegeId == c.Id))
            .Select(c => {
                var entry = manualEntries.First(e => e.CollegeId == c.Id);
                var sportResults = new Dictionary<string, SportResultDto>();
                
                if (!string.IsNullOrEmpty(entry.SportResults))
                {
                    try 
                    {
                        var manualSportResults = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(entry.SportResults);
                        if (manualSportResults != null)
                        {
                            foreach (var kvp in manualSportResults)
                            {
                                string key = kvp.Key == "Армреслінг" ? "Армрестлінг" : kvp.Key;
                                key = key == "Футзал" ? "Футбол" : key;
                                key = (key == "Баскетбол 3x3" || key == "Баскетбол 3х3") ? "Баскетбол" : key;
                                key = key == "Волейбол" ? "Волейбол (Юнаки)" : key;
                                if (sports.Contains(key))
                                {
                                    long? tId = null;
                                    if (sportToEventId.TryGetValue(key, out var id)) tId = id;
                                    sportResults[key] = new SportResultDto { Rank = kvp.Value, Points = 0, TournamentId = tId };
                                }
                            }
                        }
                    }
                    catch { /* Ignore invalid JSON */ }
                }

                // Fill missing sports with "-"
                foreach (var s in sports)
                {
                    if (!sportResults.ContainsKey(s))
                    {
                        long? tId = null;
                        if (sportToEventId.TryGetValue(s, out var id)) tId = id;
                        sportResults[s] = new SportResultDto { Rank = "-", Points = 0, TournamentId = tId };
                    }
                }

                return new GroupRatingRowDto
                {
                    CollegeId = c.Id,
                    GroupName = c.ShortName ?? c.Name,
                    SportResults = sportResults,
                    TotalPoints = entry.Points,
                    TotalRank = !string.IsNullOrWhiteSpace(entry.Rank) ? entry.Rank : "-"
                };
            }).OrderBy(r => {
                if (r.TotalRank == "-") return int.MaxValue;
                var parts = r.TotalRank!.Split('-');
                if (int.TryParse(parts[0], out int rank)) return rank;
                return int.MaxValue;
            }).ToList();

        return new SeasonRatingDto
        {
            SeasonId = season.Id,
            SeasonName = season.Name,
            Sports = sports,
            Rows = rows
        };
    }

    public async Task UpdateRegionalRatingAsync(long seasonId, UpdateManualRatingRequestDto request)
    {
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null) throw new Exception("Season not found");

        if (request.ActiveSports != null)
        {
            season.RegionalRatingSports = System.Text.Json.JsonSerializer.Serialize(request.ActiveSports);
        }

        var existingEntries = await _context.RegionalRatingManualEntries
            .Where(e => e.SeasonId == seasonId)
            .ToListAsync();

        _context.RegionalRatingManualEntries.RemoveRange(existingEntries);

        foreach (var entry in request.Entries)
        {
            string? sportResultsJson = null;
            if (entry.SportResults != null && entry.SportResults.Count > 0)
            {
                sportResultsJson = System.Text.Json.JsonSerializer.Serialize(entry.SportResults);
            }

            _context.RegionalRatingManualEntries.Add(new RegionalRatingManualEntry
            {
                SeasonId = seasonId,
                CollegeId = entry.CollegeId,
                Points = entry.Points,
                Rank = entry.Rank,
                SportResults = sportResultsJson
            });
        }

        await _context.SaveChangesAsync();
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
                EndDate = s.EndDate,
                SpartakiadNumber = s.SpartakiadNumber
            }).ToListAsync();
    }

    public async Task<SeasonDto?> GetCurrentSeasonAsync()
    {
        var season = await _context.Seasons.AsNoTracking().FirstOrDefaultAsync(s => s.IsCurrent);
        if (season == null) return null;
        return new SeasonDto
        {
            Id = season.Id,
            Name = season.Name,
            IsCurrent = season.IsCurrent,
            StartDate = season.StartDate,
            EndDate = season.EndDate,
            SpartakiadNumber = season.SpartakiadNumber
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
            EndDate = dto.EndDate,
            SpartakiadNumber = dto.SpartakiadNumber
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
        season.SpartakiadNumber = dto.SpartakiadNumber;

        
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
