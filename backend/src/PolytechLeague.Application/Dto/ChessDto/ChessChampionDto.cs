namespace PolytechLeague.Application.Dto.ChessDto;

public class ChessChampionDto
{
    public long PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string? PlayerPhotoUrl { get; set; }
    public string SeasonName { get; set; } = string.Empty;
    public string TournamentName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double Points { get; set; }
}
