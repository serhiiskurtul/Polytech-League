using PolytechLeague.Domain.Entities.Volleyball.Enums;

namespace PolytechLeague.Domain.Entities.Volleyball;

public class VolleyballEvent : Event
{
    public VolleyballTournamentType TournamentType { get; set; }
    public ICollection<VolleyballMatch>? Matches { get; set; }
}
