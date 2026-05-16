using PolytechLeague.Domain.Entities.Basketball.Enums;

namespace PolytechLeague.Domain.Entities.Basketball;

public class BasketballEvent : Event
{
    public BasketballTournamentType? TournamentType { get; set; }

    public ICollection<BasketballMatch>? Matches { get; set; }
    public ICollection<BasketballEventParticipant>? Participants { get; set; }
    public ICollection<BasketballTeam>? Teams { get; set; }
    
    public long? FirstPlaceTeamId { get; set; }
    public BasketballTeam? FirstPlaceTeam { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public BasketballTeam? SecondPlaceTeam { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public BasketballTeam? ThirdPlaceTeam { get; set; }
}
