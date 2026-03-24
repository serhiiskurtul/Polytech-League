namespace PolytechLeague.Domain.Entities.Basketball3x3;

public class Basketball3x3Team
{
    public long Id { get; set; }
    public required string Name { get; set; }
    
    public ICollection<Basketball3x3Player>? Players { get; set; }
}
