namespace PolytechLeague.Domain.Entities.TableTennis;

public class TableTennisEventParticipant
{
    public long TournamentId { get; set; }
    public TableTennisEvent? Tournament { get; set; }

    public long PlayerId { get; set; }
    public TableTennisPlayer? Player { get; set; }

    public double Points { get; set; } = 0;
    public int Rank { get; set; }
}
