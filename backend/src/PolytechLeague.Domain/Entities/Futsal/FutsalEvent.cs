namespace PolytechLeague.Domain.Entities.Futsal;

public class FutsalEvent : Event
{
    public ICollection<FutsalMatch>? Matches { get; set; }
}
