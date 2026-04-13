namespace PolytechLeague.Domain.Entities.Checkers;

public class CheckersEvent : Event
{
    public required string TimeControl { get; set; }
    public required int ParticipantsCount { get; set; }
    public required CheckersSystem System { get; set; }
    public required CheckersVariant Variant { get; set; }
    public ICollection<CheckersGame>? Games { get; set; }
    public ICollection<CheckersEventParticipant>? Players { get; set; }
    public ICollection<CheckersTeam>? Teams { get; set; }
    public long? FirstPlaceTeamId { get; set; }
    public CheckersTeam? FirstPlaceTeam { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public CheckersTeam? SecondPlaceTeam { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public CheckersTeam? ThirdPlaceTeam { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum CheckersSystem
{
    Swiss,       // Швейцарська система
    RoundRobin,  // Кругова система
    Knockout     // Олімпійська система (на вибування)
}

public enum CheckersVariant
{
    Russian,         // Російські (64 клітинки)
    International,   // Міжнародні (100 клітинок)
    English          // Англійські / Checkers (64 клітинки)
}