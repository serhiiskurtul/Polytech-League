namespace CampusLeague.Domain.Entities.Chess;

public class ChessEvent : Event
{
    public required string TimeControl { get; set; }
    public required int ParticipantsCount { get; set; }
    public required ChessSystem System { get; set; }
    public ICollection<ChessGame>? Games { get; set; }
    public ICollection<ChessEventParticipant>? Players { get; set; }
}

public enum ChessSystem
{
    Swiss, // швейцарська система
    RoundRobin, // кругова система
    Knockout // олімпійська система
}
