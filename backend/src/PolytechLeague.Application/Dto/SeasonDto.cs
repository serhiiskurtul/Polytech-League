namespace PolytechLeague.Application.Dto;

public class SeasonDto
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? SpartakiadNumber { get; set; }
}

