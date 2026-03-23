using PolytechLeague.Domain.Entities.TableTennis.Enums;

namespace PolytechLeague.Domain.Entities.TableTennis;

public class TableTennisPlayer : Player
{
    public int Rating { get; set; } = 0;
    public TableTennisHand PlayingHand { get; set; }
}
