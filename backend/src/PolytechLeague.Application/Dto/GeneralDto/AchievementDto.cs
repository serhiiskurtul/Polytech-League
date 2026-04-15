namespace PolytechLeague.Application.Dto.GeneralDto;

public class AchievementDto
{
    public required string TournamentName { get; set; }
    public required string SportType { get; set; }
    public string? Result { get; set; }
    public DateTime Date { get; set; }
    public int? Rank { get; set; }
}
