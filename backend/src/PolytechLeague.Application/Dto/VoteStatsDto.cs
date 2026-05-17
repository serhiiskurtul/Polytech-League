using System.Collections.Generic;

namespace PolytechLeague.Application.Dto;

public class VoteStatsDto
{
    public int TotalVotes { get; set; }
    public bool HasVoted { get; set; }
    public List<VoteOptionStatDto> OptionStats { get; set; } = new();
}

public class VoteOptionStatDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Votes { get; set; }
    public double Percentage { get; set; }
}
