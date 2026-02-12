namespace PolytechLeague.Domain.Entities;

public abstract class Player : Person
{
    public required string City { get; set; }
    public required string Group { get; set; }

}