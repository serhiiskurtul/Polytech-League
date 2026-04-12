using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Domain.Entities.Armwrestling;

public class ArmwrestlingMatch
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public ArmwrestlingEvent? Event { get; set; }
    
    public long Player1Id { get; set; }
    public Player? Player1 { get; set; }
    
    public long Player2Id { get; set; }
    public Player? Player2 { get; set; }
    
    public long? WinnerId { get; set; }
    public Player? Winner { get; set; }
    
    public string? Stage { get; set; } // Round of 16, Quarterfinal, etc.
}
