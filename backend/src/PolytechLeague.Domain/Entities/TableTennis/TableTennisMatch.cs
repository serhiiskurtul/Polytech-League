using PolytechLeague.Domain.Entities.TableTennis.Enums;
using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities.TableTennis;

public class TableTennisMatch
{
    public long Id { get; set; }
    
    public long TournamentId { get; set; }
    public TableTennisEvent? Tournament { get; set; }

    public long Player1Id { get; set; }
    public TableTennisPlayer? Player1 { get; set; }

    public long Player2Id { get; set; }
    public TableTennisPlayer? Player2 { get; set; }

    public long? WinnerId { get; set; }
    public TableTennisPlayer? Winner { get; set; }

    public string? Stage { get; set; }
    public int TableNumber { get; set; }
    public int SetsToWin { get; set; }

    public ICollection<TableTennisSet>? Sets { get; set; }
}
