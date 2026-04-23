using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.TableTennis.Enums;
using PolytechLeague.Domain.Entities.Armwrestling.Enums;
using PolytechLeague.Domain.Entities.Futsal.Enums;
using PolytechLeague.Domain.Entities.Volleyball.Enums;

namespace PolytechLeague.Application.Dto.GeneralDto;

public class PlayerDto
{
    public long? Id { get; set; }
    public long? UserId { get; set; }
    public string? FirstName { get; set; }
    public string? Patronymic { get; set; }
    public string? LastName { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? Group { get; set; }
    public int EnrollmentYear { get; set; }
    public int SpecialtyCode { get; set; }
    public int GroupNumber { get; set; }
    public string? StudentId { get; set; }
    public string? City { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Faculty { get; set; }
    public string? Bio { get; set; }
    public string? InstagramUrl { get; set; }

    // Chess
    public int? ChessRating { get; set; }
    public string? ChessFideId { get; set; }
    public string? ChessTitle { get; set; }

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
    public FutsalPosition? FutsalPosition { get; set; }
    public int? FutsalShirtNumber { get; set; }
    public long? FutsalTeamId { get; set; }

    public VolleyballPosition? VolleyballPosition { get; set; }
    public int? VolleyballShirtNumber { get; set; }
    public long? VolleyballTeamId { get; set; }

    public int? BasketballShirtNumber { get; set; }
    public long? BasketballTeamId { get; set; }

    public int? BeachVolleyballShirtNumber { get; set; }
    public long? BeachVolleyballTeamId { get; set; }

    public List<AchievementDto> Achievements { get; set; } = new();
    public List<string> Sports { get; set; } = new();

    // Stats
    public int MatchesPlayed { get; set; }
    public int WinsCount { get; set; }
    public int GoalsScored { get; set; }
    public int PerformanceScore { get; set; }
    public int GlobalRank { get; set; }
    public List<SportStatsDto> SportStats { get; set; } = new();
    public List<PointEntryDto> PointsBreakdown { get; set; } = new();
}

public class PointEntryDto
{
    public string Description { get; set; } = string.Empty;
    public int Points { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class SportStatsDto
{
    public string SportType { get; set; } = string.Empty;
    public int PerformanceScore { get; set; }
    public int MatchesPlayed { get; set; }
    public int WinsCount { get; set; }
    public int GoalsCount { get; set; }
    public int Rank { get; set; }
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
            BirthDate = player.BirthDate,
            Group = player.Group,
            EnrollmentYear = player.EnrollmentYear,
            SpecialtyCode = player.SpecialtyCode,
            GroupNumber = player.GroupNumber,
            StudentId = player.StudentId,
            City = player.City,
            PhotoUrl = player.PhotoUrl,
            Faculty = player.Faculty,
            Bio = player.Bio,
            InstagramUrl = player.InstagramUrl,
            ChessRating = player.ChessRating,
            ChessFideId = player.ChessFideId,
            ChessTitle = player.ChessTitle,
            CheckersRating = player.CheckersRating,
            TableTennisRating = player.TableTennisRating,
            TableTennisHand = player.TableTennisHand,
            ArmwrestlingWeight = player.ArmwrestlingWeight,
            ArmwrestlingPreferredWeightClass = player.ArmwrestlingPreferredWeightClass,
            FutsalPosition = player.FutsalPosition,
            FutsalShirtNumber = player.FutsalShirtNumber,
            FutsalTeamId = player.FutsalTeamId,
            VolleyballPosition = player.VolleyballPosition,
            VolleyballShirtNumber = player.VolleyballShirtNumber,
            VolleyballTeamId = player.VolleyballTeamId,
            BasketballShirtNumber = player.BasketballShirtNumber,
            BasketballTeamId = player.BasketballTeamId,
            BeachVolleyballShirtNumber = player.BeachVolleyballShirtNumber,
            BeachVolleyballTeamId = player.BeachVolleyballTeamId,
            Achievements = new List<AchievementDto>(),
            MatchesPlayed = 0,
            WinsCount = 0,
            GoalsScored = 0,
            PerformanceScore = 0,
            Sports = new List<string>(),
            SportStats = new List<SportStatsDto>()
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
            BirthDate = dto.BirthDate ?? DateOnly.MinValue,
            Group = dto.Group ?? string.Empty,
            EnrollmentYear = dto.EnrollmentYear,
            SpecialtyCode = dto.SpecialtyCode,
            GroupNumber = dto.GroupNumber,
            StudentId = dto.StudentId,
            City = dto.City ?? "Чернівці",
            PhotoUrl = dto.PhotoUrl ?? "",
            Faculty = dto.Faculty,
            Bio = dto.Bio,
            InstagramUrl = dto.InstagramUrl,
            ChessRating = dto.ChessRating,
            ChessFideId = dto.ChessFideId,
            ChessTitle = dto.ChessTitle,
            CheckersRating = dto.CheckersRating,
            TableTennisRating = dto.TableTennisRating,
            TableTennisHand = dto.TableTennisHand,
            ArmwrestlingWeight = dto.ArmwrestlingWeight,
            ArmwrestlingPreferredWeightClass = dto.ArmwrestlingPreferredWeightClass,
            FutsalPosition = dto.FutsalPosition,
            FutsalShirtNumber = dto.FutsalShirtNumber,
            FutsalTeamId = dto.FutsalTeamId,
            VolleyballPosition = dto.VolleyballPosition,
            VolleyballShirtNumber = dto.VolleyballShirtNumber,
            VolleyballTeamId = dto.VolleyballTeamId,
            BasketballShirtNumber = dto.BasketballShirtNumber,
            BasketballTeamId = dto.BasketballTeamId,
            BeachVolleyballShirtNumber = dto.BeachVolleyballShirtNumber,
            BeachVolleyballTeamId = dto.BeachVolleyballTeamId
        };
    }
}
