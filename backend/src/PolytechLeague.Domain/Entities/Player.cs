using PolytechLeague.Domain.Entities.TableTennis.Enums;
using PolytechLeague.Domain.Entities.Armwrestling.Enums;
using PolytechLeague.Domain.Entities.Futsal.Enums;
using PolytechLeague.Domain.Entities.Volleyball.Enums;

namespace PolytechLeague.Domain.Entities;

public class Player : Person
{
    public required string City { get; set; }
    public required string Group { get; set; }
    public int EnrollmentYear { get; set; }
    public int SpecialtyCode { get; set; }
    public int GroupNumber { get; set; }
    public string? StudentId { get; set; }
    public string? StudyYears { get; set; }
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
    public ArmwrestlingWeightClass? ArmwrestlingPreferredWeightClass { get; set; }

    // Team Sports Metadata 
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

    public bool IsExternal { get; set; } = false;
    public string? PreviousNames { get; set; }
    public long? CollegeId { get; set; }
    public College? College { get; set; }
}