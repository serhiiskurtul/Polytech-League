namespace PolytechLeague.Application.Dto;

public class RecentMatchDto
{
    public long Id { get; set; }
    public string Sport { get; set; } = string.Empty;
    public string Team1Name { get; set; } = string.Empty;
    public string Team2Name { get; set; } = string.Empty;
    public string? Score1 { get; set; }
    public string? Score2 { get; set; }
    public string Status { get; set; } = "Completed";
    public string Date { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string TournamentName { get; set; } = string.Empty;
    public long TournamentId { get; set; }
}
