using PolytechLeague.Domain.Entities.Futsal.Enums;

namespace PolytechLeague.Domain.Entities.Futsal;

public class FutsalEvent : Event
{
    public FutsalTournamentType TournamentType { get; set; }
    public ICollection<FutsalMatch>? Matches { get; set; }
}
