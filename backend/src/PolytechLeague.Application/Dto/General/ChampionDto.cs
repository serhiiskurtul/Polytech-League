namespace PolytechLeague.Application.Dto.General;

public class ChampionDto
{
    public long PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string? PlayerPhotoUrl { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public long TournamentId { get; set; }
    public string TournamentName { get; set; } = string.Empty;
    public string SportType { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double Points { get; set; }
}
