using PolytechLeague.Domain.Entities.BeachVolleyball.Enums;

namespace PolytechLeague.Domain.Entities.BeachVolleyball;

public class BeachVolleyballEvent : Event
{
    public BeachVolleyballTournamentType TournamentType { get; set; }
    public ICollection<BeachVolleyballMatch>? Matches { get; set; }
}
