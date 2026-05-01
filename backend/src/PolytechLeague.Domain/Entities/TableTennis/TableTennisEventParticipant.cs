using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities.TableTennis;

public class TableTennisEventParticipant
{
    public long TournamentId { get; set; }
    public TableTennisEvent? Tournament { get; set; }

    public long PlayerId { get; set; }
    public Player? Player { get; set; }

    public double Points { get; set; } = 0;
    public int Rank { get; set; }
    public string? GroupName { get; set; }
    public string? PlayerFirstName { get; set; }
    public string? PlayerLastName { get; set; }
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Approved;
    public long? CollegeId { get; set; }
    public College? College { get; set; }
}
