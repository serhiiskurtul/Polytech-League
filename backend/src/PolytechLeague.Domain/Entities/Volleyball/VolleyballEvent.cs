using PolytechLeague.Domain.Entities.Volleyball.Enums;

namespace PolytechLeague.Domain.Entities.Volleyball;

public class VolleyballEvent : Event
{
    public VolleyballTournamentType? TournamentType { get; set; }

    public ICollection<VolleyballMatch>? Matches { get; set; }
    public ICollection<VolleyballTeam>? Teams { get; set; }

    public long? FirstPlaceTeamId { get; set; }
    public VolleyballTeam? FirstPlaceTeam { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public VolleyballTeam? SecondPlaceTeam { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public VolleyballTeam? ThirdPlaceTeam { get; set; }
}
