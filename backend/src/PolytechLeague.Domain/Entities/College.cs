namespace PolytechLeague.Domain.Entities;

public class College
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string ShortName { get; set; }
    public string? LogoUrl { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
}
