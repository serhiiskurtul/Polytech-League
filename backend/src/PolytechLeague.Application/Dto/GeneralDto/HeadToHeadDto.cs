using PolytechLeague.Application.Dto.GeneralDto;

namespace PolytechLeague.Application.Dto.GeneralDto;

public class HeadToHeadDto
{
    public long Player1Id { get; set; }
    public long Player2Id { get; set; }
    public int Player1Wins { get; set; }
    public int Player2Wins { get; set; }
    public int Draws { get; set; }
    public List<HeadToHeadMatchDto> Matches { get; set; } = new();
}

public class HeadToHeadMatchDto
{
    public string SportType { get; set; } = string.Empty;
    public string TournamentName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Score { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty; // e.g. "P1 Won", "P2 Won", "Draw"
    public long TournamentId { get; set; }
}
