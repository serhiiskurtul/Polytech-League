using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.TableTennis.Enums;
using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities.TableTennis;

public class TableTennisMatch
{
    public long Id { get; set; }
    
    public long TournamentId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public TableTennisEvent? Tournament { get; set; }

    public long Player1Id { get; set; }
    public Player? Player1 { get; set; }

    public long Player2Id { get; set; }
    public Player? Player2 { get; set; }

    public long? WinnerId { get; set; }
    public Player? Winner { get; set; }

    public string? Stage { get; set; }
    public int TableNumber { get; set; }
    public int SetsToWin { get; set; }

    public ICollection<TableTennisSet>? Sets { get; set; }
}
