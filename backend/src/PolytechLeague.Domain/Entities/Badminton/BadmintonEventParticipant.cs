namespace PolytechLeague.Domain.Entities.Badminton;

public class BadmintonEventParticipant
{
    public long TournamentId { get; set; }
    public BadmintonEvent? Tournament { get; set; }

    public long PlayerId { get; set; }
    public BadmintonPlayer? Player { get; set; }

    public double Points { get; set; } = 0;
    public int Rank { get; set; }
}
