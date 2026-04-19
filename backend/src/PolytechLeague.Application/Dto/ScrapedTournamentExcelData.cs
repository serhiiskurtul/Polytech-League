namespace PolytechLeague.Application.Dto;

public class ScrapedTournamentExcelData
{
    public string? TournamentName { get; set; }
    public List<ExcelPlayer> Players { get; set; } = new();
}

public class ExcelPlayer
{
    public string FullName { get; set; } = string.Empty;
    public string? FideId { get; set; }
    public int? Rating { get; set; }
    public double Points { get; set; }
    public int? Rank { get; set; }
    public string? Title { get; set; }
    public string? Group { get; set; }
}
