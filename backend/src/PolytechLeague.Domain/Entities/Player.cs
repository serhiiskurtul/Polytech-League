using PolytechLeague.Domain.Entities.TableTennis.Enums;
using PolytechLeague.Domain.Entities.Armwrestling.Enums;
using PolytechLeague.Domain.Entities.Football.Enums;
using PolytechLeague.Domain.Entities.Football;
using PolytechLeague.Domain.Entities.Volleyball;
using PolytechLeague.Domain.Entities.Volleyball.Enums;
using PolytechLeague.Domain.Entities.Enums;
using PolytechLeague.Domain.Entities.BeachVolleyball;
using PolytechLeague.Domain.Entities.Basketball;
using PolytechLeague.Domain.Entities.Chess;
using PolytechLeague.Domain.Entities.Checkers;

namespace PolytechLeague.Domain.Entities;

public class Player : Person
{
    public required string City { get; set; }
    public string? Group { get; set; }
    public bool IsGuest { get; set; } = false;
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
    public ICollection<ChessTeam>? ChessTeams { get; set; }

    // Checkers
    public int? CheckersRating { get; set; }
    public ICollection<CheckersTeam>? CheckersTeams { get; set; }

    // Table Tennis
    public int? TableTennisRating { get; set; }
    public TableTennisHand? TableTennisHand { get; set; }

    // Armwrestling
    public float? ArmwrestlingWeight { get; set; }
    public ArmwrestlingWeightClass? ArmwrestlingPreferredWeightClass { get; set; }

    // Team Sports Metadata 
    public FootballPosition? FootballPosition { get; set; }
    public int? FootballShirtNumber { get; set; }
    public ICollection<FootballTeam>? FootballTeams { get; set; }

    public VolleyballPosition? VolleyballPosition { get; set; }
    public int? VolleyballShirtNumber { get; set; }
    public ICollection<VolleyballTeam>? VolleyballTeams { get; set; }

    public int? BasketballShirtNumber { get; set; }
    public ICollection<BasketballTeam>? BasketballTeams { get; set; }

    public int? BeachVolleyballShirtNumber { get; set; }
    public ICollection<BeachVolleyballTeam>? BeachVolleyballTeams { get; set; }

    public bool IsExternal { get; set; } = false;
    public string? PreviousNames { get; set; }
    public bool IsFounder { get; set; } = false;
    public long? CollegeId { get; set; }
    public College? College { get; set; }
    public Gender Gender { get; set; } = Gender.None;
}
