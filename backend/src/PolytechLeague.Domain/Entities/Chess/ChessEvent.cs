namespace PolytechLeague.Domain.Entities.Chess;

public class ChessEvent : Event
{
    public required string TimeControl { get; set; }
    public required int ParticipantsCount { get; set; }
    public required ChessSystem System { get; set; }
    public ICollection<ChessGame>? Games { get; set; }
    public ICollection<ChessEventParticipant>? Players { get; set; }
    public ICollection<ChessTeam>? Teams { get; set; }
    public long? FirstPlaceTeamId { get; set; }
    public ChessTeam? FirstPlaceTeam { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public ChessTeam? SecondPlaceTeam { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public ChessTeam? ThirdPlaceTeam { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum ChessSystem
{
    Swiss, // швейцарська система
    RoundRobin, // кругова система
    Knockout // олімпійська система
}
