namespace PolytechLeague.Application.Dto.GeneralDto;

public class PlayerRankResponse
{
    public int GlobalRank { get; set; }
    public int PerformanceScore { get; set; }
    public List<SportStatsDto> SportStats { get; set; } = new();
}