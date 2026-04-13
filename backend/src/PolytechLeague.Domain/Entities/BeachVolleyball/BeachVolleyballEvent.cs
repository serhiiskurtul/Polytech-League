using PolytechLeague.Domain.Entities.BeachVolleyball.Enums;

namespace PolytechLeague.Domain.Entities.BeachVolleyball;

public class BeachVolleyballEvent : Event
{
    public BeachVolleyballTournamentType TournamentType { get; set; }
    public ICollection<BeachVolleyballMatch>? Matches { get; set; }
    public ICollection<BeachVolleyballTeam>? Teams { get; set; }

    public long? FirstPlaceTeamId { get; set; }
    public BeachVolleyballTeam? FirstPlaceTeam { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public BeachVolleyballTeam? SecondPlaceTeam { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public BeachVolleyballTeam? ThirdPlaceTeam { get; set; }
}
