namespace PolytechLeague.Domain.Entities;

public abstract class Person
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public User? User { get; set; }
    public required string FirstName { get; set; }
    public string? Patronymic { get; set; }
    public required string LastName { get; set; }
    public required string PhotoUrl { get; set; }
}
