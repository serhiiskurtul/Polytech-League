using PolytechLeague.Domain.Entities.Basketball3x3.Enums;

namespace PolytechLeague.Domain.Entities.Basketball3x3;

public class Basketball3x3Event : Event
{
    public Basketball3x3TournamentType TournamentType { get; set; }
    public ICollection<Basketball3x3Match>? Matches { get; set; }
}
