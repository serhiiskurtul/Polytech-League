namespace PolytechLeague.Domain.Entities.Futsal;

public class FutsalTeam
{
    public long Id { get; set; }
    public required string Name { get; set; }
    
    public ICollection<FutsalPlayer>? Players { get; set; }
}
