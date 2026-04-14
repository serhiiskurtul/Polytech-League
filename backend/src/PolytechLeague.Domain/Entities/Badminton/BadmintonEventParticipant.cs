using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Badminton;

public class BadmintonEventParticipant
{
    public long EventId { get; set; }
    public BadmintonEvent? Event { get; set; }

    public long PlayerId { get; set; }
    public Player? Player { get; set; }

    public double Points { get; set; } = 0;
    public int Rank { get; set; }
}
