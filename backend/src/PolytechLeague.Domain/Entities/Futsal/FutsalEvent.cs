using PolytechLeague.Domain.Entities.Futsal.Enums;

namespace PolytechLeague.Domain.Entities.Futsal;

public class FutsalEvent : Event
{
    public FutsalTournamentType? TournamentType { get; set; }

    public ICollection<FutsalMatch>? Matches { get; set; }
    public ICollection<FutsalEventParticipant>? Participants { get; set; }
    public ICollection<FutsalTeam>? Teams { get; set; }
    
    public long? FirstPlaceTeamId { get; set; }
    public FutsalTeam? FirstPlaceTeam { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public FutsalTeam? SecondPlaceTeam { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public FutsalTeam? ThirdPlaceTeam { get; set; }
}
