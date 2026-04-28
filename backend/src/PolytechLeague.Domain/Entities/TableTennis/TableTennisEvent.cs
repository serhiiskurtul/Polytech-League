using PolytechLeague.Domain.Entities.TableTennis.Enums;

namespace PolytechLeague.Domain.Entities.TableTennis;

public class TableTennisEvent : Event
{
    public TableTennisTournamentType? TournamentType { get; set; }

    public ICollection<TableTennisMatch>? Matches { get; set; }
    public ICollection<TableTennisEventParticipant>? Participants { get; set; }
    public ICollection<TableTennisTeam>? Teams { get; set; }

    public long? FirstPlaceTeamId { get; set; }
    public TableTennisTeam? FirstPlaceTeam { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public TableTennisTeam? SecondPlaceTeam { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public TableTennisTeam? ThirdPlaceTeam { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

