using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.TableTennis.Enums;
using PolytechLeague.Domain.Entities.Armwrestling.Enums;
using PolytechLeague.Domain.Entities.Football.Enums;
using PolytechLeague.Domain.Entities.Volleyball.Enums;
using PolytechLeague.Domain.Entities.Enums;

namespace PolytechLeague.Application.Dto.GeneralDto;

public class PlayerDto
{
    public long? Id { get; set; }
    public long? UserId { get; set; }
    public string? FirstName { get; set; }
    public string? Patronymic { get; set; }
    public string? LastName { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Gender Gender { get; set; }
    public string? Group { get; set; }
    public int EnrollmentYear { get; set; }
    public int SpecialtyCode { get; set; }
    public int GroupNumber { get; set; }
    public string? StudentId { get; set; }
    public string? City { get; set; }
    public string? PhotoUrl { get; set; }
    public string? StudyYears { get; set; }
    public string? Bio { get; set; }
    public string? InstagramUrl { get; set; }
    public string? PreviousNames { get; set; }
    public bool IsFounder { get; set; }
    public List<AutomaticBadgeDto> AutomaticBadges { get; set; } = new();
    public bool HasPendingMergeRequest { get; set; }
    public string? UserEmail { get; set; }

    // Chess
    public int? ChessRating { get; set; }
    public string? ChessFideId { get; set; }
    public string? ChessTitle { get; set; }
    public bool HasChessFourStreak { get; set; }
    public double? ChessTotalPoints { get; set; }
    public double? ChessTotalPossiblePoints { get; set; }
    public bool IsChessUndefeatedFourSeasons { get; set; }

    // Checkers
    public int? CheckersRating { get; set; }

    // Table Tennis
    public int? TableTennisRating { get; set; }
    public TableTennisHand? TableTennisHand { get; set; }

    // Armwrestling
    public float? ArmwrestlingWeight { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ArmwrestlingWeightClass? ArmwrestlingPreferredWeightClass { get; set; }
    [JsonPropertyName("armwrestlingWeightClasses")]
    public List<ArmwrestlingWeightClass> ArmwrestlingWeightClasses { get; set; } = new();

    // Team Sports
    public FootballPosition? FootballPosition { get; set; }
    public int? FootballShirtNumber { get; set; }
    public long? FootballTeamId { get; set; }

    public VolleyballPosition? VolleyballPosition { get; set; }
    public int? VolleyballShirtNumber { get; set; }
    public long? VolleyballTeamId { get; set; }

    public int? BasketballShirtNumber { get; set; }
    public long? BasketballTeamId { get; set; }

    public int? BeachVolleyballShirtNumber { get; set; }
    public long? BeachVolleyballTeamId { get; set; }

    public bool IsExternal { get; set; }
    public bool IsGuest { get; set; }

    public List<AchievementDto> Achievements { get; set; } = new();
    public List<string> Sports { get; set; } = new();

    // Stats
    public int MatchesPlayed { get; set; }
    public int WinsCount { get; set; }
    public int GoalsCount { get; set; }
    public int FootballCollegeGroupGoals { get; set; }
    public int FootballRegionalGoals { get; set; }
    public int FootballNationalGoals { get; set; }
    public int HatTricksCount { get; set; }
    public int PokersCount { get; set; }
    public int PentatricksCount { get; set; }
    public int FinalGoalsCount { get; set; }
    public int SemifinalGoalsCount { get; set; }
    public int FinalMatchesCount { get; set; }
    public int SemifinalMatchesCount { get; set; }
    public int YellowCardsCount { get; set; }
    public int RedCardsCount { get; set; }
    public int PerformanceScore { get; set; }
    public int GlobalRank { get; set; }
    public int CompetitionsCount { get; set; }
    public int MvpCount { get; set; }
    public List<SportStatsDto> SportStats { get; set; } = new();

    public List<PointEntryDto> PointsBreakdown { get; set; } = new();
    public List<FootballGoalMatchDto> FootballGoalMatches { get; set; } = new();
}

public class FootballGoalMatchDto
{
    public long MatchId { get; set; }
    public long TournamentId { get; set; }
    public string TournamentName { get; set; } = string.Empty;
    public string HomeTeamName { get; set; } = string.Empty;
    public string AwayTeamName { get; set; } = string.Empty;
    public int GoalsScored { get; set; }
    public DateTime Date { get; set; }
}


public class PointEntryDto
{
    public string Description { get; set; } = string.Empty;
    public int Points { get; set; }
    public string Category { get; set; } = string.Empty;
    public string SportType { get; set; } = string.Empty;
}

public class SportStatsDto
{
    public string SportType { get; set; } = string.Empty;
    public int PerformanceScore { get; set; }
    public int MatchesPlayed { get; set; }
    public int WinsCount { get; set; }
    public int GoalsCount { get; set; }
    public int YellowCardsCount { get; set; }
    public int RedCardsCount { get; set; }
    public int Rank { get; set; }
}

public class AutomaticBadgeDto
{
    public required string Type { get; set; } // e.g. "TopScorer", "SeasonTopScorer", "RankLeader"
    public required string Label { get; set; }
    public string? SeasonName { get; set; }
    public string? Icon { get; set; }
}

public static class PlayerDtoExtensions
{
    public static PlayerDto ToDto(this Player player)
    {
        return new PlayerDto
        {
            Id = player.Id,
            UserId = player.UserId,
            FirstName = player.FirstName,
            Patronymic = player.Patronymic,
            LastName = player.LastName,
            Group = player.Group,
            EnrollmentYear = player.EnrollmentYear,
            SpecialtyCode = player.SpecialtyCode,
            GroupNumber = player.GroupNumber,
            StudentId = player.StudentId,
            City = player.City,
            PhotoUrl = player.PhotoUrl,
            StudyYears = player.StudyYears,
            Bio = player.Bio,
            InstagramUrl = player.InstagramUrl,
            PreviousNames = player.PreviousNames,
            IsFounder = player.IsFounder,
            UserEmail = player.User?.Email,
            ChessRating = player.ChessRating,
            ChessFideId = player.ChessFideId,
            ChessTitle = player.ChessTitle,
            HasChessFourStreak = false, // Will be populated in service
            ChessTotalPoints = null,
            ChessTotalPossiblePoints = null,
            IsChessUndefeatedFourSeasons = false,
            CheckersRating = player.CheckersRating,
            TableTennisRating = player.TableTennisRating,
            TableTennisHand = player.TableTennisHand,
            ArmwrestlingWeight = player.ArmwrestlingWeight,
            ArmwrestlingPreferredWeightClass = player.ArmwrestlingPreferredWeightClass,
            FootballPosition = player.FootballPosition,
            FootballShirtNumber = player.FootballShirtNumber,
            FootballTeamId = player.FootballTeamId,
            VolleyballPosition = player.VolleyballPosition,
            VolleyballShirtNumber = player.VolleyballShirtNumber,
            VolleyballTeamId = player.VolleyballTeamId,
            BasketballShirtNumber = player.BasketballShirtNumber,
            BasketballTeamId = player.BasketballTeamId,
            BeachVolleyballShirtNumber = player.BeachVolleyballShirtNumber,
            BeachVolleyballTeamId = player.BeachVolleyballTeams?.FirstOrDefault()?.Id,
            IsExternal = player.IsExternal,
            IsGuest = player.IsGuest,
            Gender = player.Gender,
            Achievements = new List<AchievementDto>(),
            MatchesPlayed = 0,
            WinsCount = 0,
            GoalsCount = 0,
            FootballCollegeGroupGoals = 0,
            FootballRegionalGoals = 0,
            FootballNationalGoals = 0,
            HatTricksCount = 0,
            PokersCount = 0,
            PentatricksCount = 0,
            FinalGoalsCount = 0,
            SemifinalGoalsCount = 0,
            FinalMatchesCount = 0,
            SemifinalMatchesCount = 0,
            YellowCardsCount = 0,
            RedCardsCount = 0,
            PerformanceScore = 0,
            Sports = new List<string>(),
            CompetitionsCount = 0,
            SportStats = new List<SportStatsDto>(),
            FootballGoalMatches = new List<FootballGoalMatchDto>()
        };
    }

    public static Player FromDto(this PlayerDto dto)
    {
        return new Player
        {
            Id = dto.Id ?? 0,
            UserId = dto.UserId,
            FirstName = dto.FirstName ?? string.Empty,
            Patronymic = dto.Patronymic,
            LastName = dto.LastName ?? string.Empty,
            Group = dto.Group ?? string.Empty,
            EnrollmentYear = dto.EnrollmentYear,
            SpecialtyCode = dto.SpecialtyCode,
            GroupNumber = dto.GroupNumber,
            StudentId = dto.StudentId,
            City = dto.City ?? "Чернівці",
            PhotoUrl = dto.PhotoUrl ?? "",
            StudyYears = dto.StudyYears,
            Bio = dto.Bio,
            InstagramUrl = dto.InstagramUrl,
            PreviousNames = dto.PreviousNames,
            IsFounder = dto.IsFounder,
            ChessRating = dto.ChessRating,
            ChessFideId = dto.ChessFideId,
            ChessTitle = dto.ChessTitle,
            CheckersRating = dto.CheckersRating,
            TableTennisRating = dto.TableTennisRating,
            TableTennisHand = dto.TableTennisHand,
            ArmwrestlingWeight = dto.ArmwrestlingWeight,
            ArmwrestlingPreferredWeightClass = dto.ArmwrestlingPreferredWeightClass,
            FootballPosition = dto.FootballPosition,
            FootballShirtNumber = dto.FootballShirtNumber,
            FootballTeamId = dto.FootballTeamId,
            VolleyballPosition = dto.VolleyballPosition,
            VolleyballShirtNumber = dto.VolleyballShirtNumber,
            VolleyballTeamId = dto.VolleyballTeamId,
            BasketballShirtNumber = dto.BasketballShirtNumber,
            BasketballTeamId = dto.BasketballTeamId,
            BeachVolleyballShirtNumber = dto.BeachVolleyballShirtNumber,
            IsExternal = dto.IsExternal,
            IsGuest = dto.IsGuest,
            Gender = dto.Gender
        };
    }
}

