namespace PolytechLeague.Application.Dto;

public class GeneralStatsDto
{
    public int ActiveTournamentsCount { get; set; }
    public int RegisteredPlayersCount { get; set; }
    public int TotalMatchesCount { get; set; }
    public int SeasonProgress { get; set; }
    public string TopSport { get; set; } = "Футбол";
}
