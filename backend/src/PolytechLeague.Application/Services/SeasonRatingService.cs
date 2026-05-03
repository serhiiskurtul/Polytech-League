using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services;

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
        var chessGroups = await _context.ChessEventParticipants.Select(p => p.GroupName).Distinct().ToListAsync();
        var checkersGroups = await _context.CheckersEventParticipants.Select(p => p.GroupName).Distinct().ToListAsync();
        var armGroups = await _context.ArmwrestlingEventParticipants.Select(p => p.GroupName).Distinct().ToListAsync();
        var ttGroups = await _context.TableTennisEventParticipants.Select(p => p.GroupName).Distinct().ToListAsync();
        var badGroups = await _context.BadmintonEventParticipants.Select(p => p.GroupName).Distinct().ToListAsync();

        var allGroups = playerGroups
            .Union(futsalGroups)
            .Union(volleyballGroups)
            .Union(basketballGroups)
            .Union(beachVolleyballGroups)
            .Union(chessGroups)
            .Union(checkersGroups)
            .Union(armGroups)
            .Union(ttGroups)
            .Union(badGroups)
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
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Players!)
                .ThenInclude(p => p.Player)
            .ToListAsync();
        foreach (var e in chessEvents)
        {
            foreach (var p in e.Players ?? new List<Domain.Entities.Chess.ChessEventParticipant>())
            {
                var groupName = p.GroupName ?? p.Player?.Group;
                if (string.IsNullOrEmpty(groupName)) continue;
                var normalizedPGroup = groupName.Length >= 3 ? groupName.Substring(0, 3) : groupName;
                
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedPGroup, StringComparison.OrdinalIgnoreCase));
                if (row != null)
                {
                    int rank = p.Rank > 0 ? p.Rank : 4;
                    if (row.SportResults["Шахи"].Rank == "-" || rank < int.Parse(row.SportResults["Шахи"].Rank!))
                    {
                        row.SportResults["Шахи"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                    }
                }
            }
        }

        // 2. Checkers
        var checkersEvents = await _context.CheckersEvents
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Players!)
                .ThenInclude(p => p.Player)
            .ToListAsync();
        foreach (var e in checkersEvents)
        {
            foreach (var p in e.Players ?? new List<Domain.Entities.Checkers.CheckersEventParticipant>())
            {
                var groupName = p.GroupName ?? p.Player?.Group;
                if (string.IsNullOrEmpty(groupName)) continue;
                var normalizedPGroup = groupName.Length >= 3 ? groupName.Substring(0, 3) : groupName;
                
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedPGroup, StringComparison.OrdinalIgnoreCase));
                if (row != null)
                {
                    int rank = p.Rank > 0 ? p.Rank : 4;
                    if (row.SportResults["Шашки"].Rank == "-" || rank < int.Parse(row.SportResults["Шашки"].Rank!))
                    {
                        row.SportResults["Шашки"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                    }
                }
            }
        }

        // 3. Futsal
        var futsalEvents = await _context.FutsalEvents
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Teams)
            .ToListAsync();
        foreach (var e in futsalEvents)
        {
            foreach (var t in e.Teams ?? new List<Domain.Entities.Futsal.FutsalTeam>())
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
                    
                    if (row.SportResults["Футзал"].Rank == "-" || rank < int.Parse(row.SportResults["Футзал"].Rank!))
                    {
                        row.SportResults["Футзал"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                    }
                }
            }
        }

        // 4. Armwrestling
        var armEvents = await _context.ArmwrestlingEvents
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Players!)
                .ThenInclude(p => p.Player)
            .ToListAsync();
        foreach (var e in armEvents)
        {
            foreach (var p in e.Players ?? new List<Domain.Entities.Armwrestling.ArmwrestlingEventParticipant>())
            {
                var groupName = p.GroupName ?? p.Player?.Group;
                if (string.IsNullOrEmpty(groupName)) continue;
                var normalizedPGroup = groupName.Length >= 3 ? groupName.Substring(0, 3) : groupName;
                
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedPGroup, StringComparison.OrdinalIgnoreCase));
                if (row != null)
                {
                    int rank = p.Rank > 0 ? p.Rank : 4;
                    if (row.SportResults["Армреслінг"].Rank == "-" || rank < int.Parse(row.SportResults["Армреслінг"].Rank!))
                    {
                        row.SportResults["Армреслінг"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                    }
                }
            }
        }

        // 5. Table Tennis
        var ttEvents = await _context.TableTennisEvents
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Participants!)
                .ThenInclude(p => p.Player)
            .ToListAsync();
        foreach (var e in ttEvents)
        {
            foreach (var p in e.Participants ?? new List<Domain.Entities.TableTennis.TableTennisEventParticipant>())
            {
                var groupName = p.GroupName ?? p.Player?.Group;
                if (string.IsNullOrEmpty(groupName)) continue;
                var normalizedPGroup = groupName.Length >= 3 ? groupName.Substring(0, 3) : groupName;
                
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedPGroup, StringComparison.OrdinalIgnoreCase));
                if (row != null)
                {
                    int rank = p.Rank > 0 ? p.Rank : 4;
                    if (row.SportResults["Настільний теніс"].Rank == "-" || rank < int.Parse(row.SportResults["Настільний теніс"].Rank!))
                    {
                        row.SportResults["Настільний теніс"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                    }
                }
            }
        }

        // 6. Badminton
        var badEvents = await _context.BadmintonEvents
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Participants!)
                .ThenInclude(p => p.Player)
            .ToListAsync();
        foreach (var e in badEvents)
        {
            foreach (var p in e.Participants ?? new List<Domain.Entities.Badminton.BadmintonEventParticipant>())
            {
                var groupName = p.GroupName ?? p.Player?.Group;
                if (string.IsNullOrEmpty(groupName)) continue;
                var normalizedPGroup = groupName.Length >= 3 ? groupName.Substring(0, 3) : groupName;
                
                var row = rows.FirstOrDefault(r => r.GroupName.Equals(normalizedPGroup, StringComparison.OrdinalIgnoreCase));
                if (row != null)
                {
                    int rank = p.Rank > 0 ? p.Rank : 4;
                    if (row.SportResults["Бадмінтон"].Rank == "-" || rank < int.Parse(row.SportResults["Бадмінтон"].Rank!))
                    {
                        row.SportResults["Бадмінтон"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                    }
                }
            }
        }

        // 7. Volleyball
        var volEvents = await _context.VolleyballEvents
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
        var bballEvents = await _context.Basketball3x3Events
            .Where(e => (e.SeasonId == seasonId || (e.SeasonId == null && e.Date >= season.StartDate && e.Date <= season.EndDate)) 
                        && e.Level == CompetitionLevel.CollegeGroup)
            .Include(e => e.Teams)
            .ToListAsync();
        foreach (var e in bballEvents)
        {
            foreach (var t in e.Teams ?? new List<Domain.Entities.Basketball3x3.Basketball3x3Team>())
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
                    
                    if (row.SportResults["Баскетбол 3x3"].Rank == "-" || rank < int.Parse(row.SportResults["Баскетбол 3x3"].Rank!))
                    {
                        row.SportResults["Баскетбол 3x3"] = new SportResultDto { Rank = rank.ToString(), Points = rank };
                    }
                }
            }
        }

        // 9. Beach Volleyball
        var bvolEvents = await _context.BeachVolleyballEvents
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

        // Calculate Totals and use "total registered teams + 1" penalty per sport for those who didn't participate
        int penaltyForNonParticipation = rows.Count + 1;

        foreach (var sport in sports)
        {
            // Only apply penalty if at least one team participated in this sport during this season
            bool anyParticipation = rows.Any(r => r.SportResults[sport].Rank != "-");
            if (anyParticipation)
            {
                foreach (var row in rows)
                {
                    if (row.SportResults[sport].Rank == "-")
                    {
                        row.SportResults[sport].Points = penaltyForNonParticipation;
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

        var sports = new List<string> 
        { 
            "Шахи", "Шашки", "Футзал", "Армреслінг", 
            "Настільний теніс", "Бадмінтон", "Волейбол", 
            "Баскетбол 3x3", "Пляжний волейбол" 
        };

        var colleges = await _context.Colleges.ToListAsync();
        var manualEntries = await _context.NationalRatingManualEntries
            .Where(e => e.SeasonId == seasonId)
            .ToListAsync();

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
                                sportResults[kvp.Key] = new SportResultDto { Rank = kvp.Value, Points = 0 };
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
                        sportResults[s] = new SportResultDto { Rank = "-", Points = 0 };
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

    public async Task UpdateNationalRatingAsync(long seasonId, List<ManualRatingEntryDto> entries)
    {
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null) throw new Exception("Season not found");

        var existingEntries = await _context.NationalRatingManualEntries
            .Where(e => e.SeasonId == seasonId)
            .ToListAsync();

        _context.NationalRatingManualEntries.RemoveRange(existingEntries);

        foreach (var entry in entries)
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

        var sports = new List<string> 
        { 
            "Шахи", "Шашки", "Футзал", "Армреслінг", 
            "Настільний теніс", "Бадмінтон", "Волейбол", 
            "Баскетбол 3x3", "Пляжний волейбол" 
        };

        var colleges = await _context.Colleges.ToListAsync();
        var manualEntries = await _context.RegionalRatingManualEntries
            .Where(e => e.SeasonId == seasonId)
            .ToListAsync();

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
                                sportResults[kvp.Key] = new SportResultDto { Rank = kvp.Value, Points = 0 };
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
                        sportResults[s] = new SportResultDto { Rank = "-", Points = 0 };
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

    public async Task UpdateRegionalRatingAsync(long seasonId, List<ManualRatingEntryDto> entries)
    {
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null) throw new Exception("Season not found");

        var existingEntries = await _context.RegionalRatingManualEntries
            .Where(e => e.SeasonId == seasonId)
            .ToListAsync();

        _context.RegionalRatingManualEntries.RemoveRange(existingEntries);

        foreach (var entry in entries)
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
        var season = await _context.Seasons.FirstOrDefaultAsync(s => s.IsCurrent);
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
