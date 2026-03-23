using PolytechLeague.Domain.Entities.TableTennis.Enums;

namespace PolytechLeague.Domain.Entities.TableTennis;

public class TableTennisEvent : Event
{
    public TableTennisTournamentType TournamentType { get; set; }
    public TableTennisCategory Category { get; set; }
    
    public ICollection<TableTennisMatch>? Matches { get; set; }
    public ICollection<TableTennisEventParticipant>? Participants { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
