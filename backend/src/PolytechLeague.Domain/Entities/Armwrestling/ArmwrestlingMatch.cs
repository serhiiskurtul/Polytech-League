namespace PolytechLeague.Domain.Entities.Armwrestling;

public class ArmwrestlingMatch
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public ArmwrestlingEvent? Event { get; set; }
    
    public long Player1Id { get; set; }
    public ArmwrestlingPlayer? Player1 { get; set; }
    
    public long Player2Id { get; set; }
    public ArmwrestlingPlayer? Player2 { get; set; }
    
    public long? WinnerId { get; set; }
    public ArmwrestlingPlayer? Winner { get; set; }
    
    public string? Stage { get; set; } // Round of 16, Quarterfinal, etc.
}
