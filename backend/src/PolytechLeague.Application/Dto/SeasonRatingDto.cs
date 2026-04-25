namespace PolytechLeague.Application.Dto;

public class SeasonRatingDto
{
    public long SeasonId { get; set; }
    public required string SeasonName { get; set; }
    public required List<string> Sports { get; set; }
    public required List<GroupRatingRowDto> Rows { get; set; }
}

public class GroupRatingRowDto
{
    public required string GroupName { get; set; }
    public required Dictionary<string, SportResultDto> SportResults { get; set; }
    public int TotalPoints { get; set; }
    public int TotalRank { get; set; }
}

public class SportResultDto
{
    public int Points { get; set; } // Очки (usually same as rank)
    public int Rank { get; set; }   // Місце
}
