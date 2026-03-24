namespace PolytechLeague.Domain.Entities.Badminton;

public class BadmintonMatch
{
    public long Id { get; set; }
    
    public long TournamentId { get; set; }
    public BadmintonEvent? Tournament { get; set; }

    public long Player1Id { get; set; }
    public BadmintonPlayer? Player1 { get; set; }

    public long Player2Id { get; set; }
    public BadmintonPlayer? Player2 { get; set; }

    public long? WinnerId { get; set; }
    public BadmintonPlayer? Winner { get; set; }

    public string? Stage { get; set; }
    public int CourtNumber { get; set; }
    public int SetsToWin { get; set; } = 2; // Usually best of 3

    public ICollection<BadmintonSet>? Sets { get; set; }
}
