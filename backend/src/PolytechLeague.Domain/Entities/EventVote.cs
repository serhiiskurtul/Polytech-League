using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolytechLeague.Domain.Entities;

public class EventVote
{
    public long Id { get; set; }

    public long EventId { get; set; }
    public Event? Event { get; set; }

    public long UserId { get; set; }
    public User? User { get; set; }

    public long? VotedForTeamId { get; set; }
    // We don't add navigation property to specific sport teams because they are in different tables.
    // We will just store the ID and handle it in the application layer.

    public long? VotedForPlayerId { get; set; }
    public Player? VotedForPlayer { get; set; }

    public DateTime VotedAt { get; set; } = DateTime.UtcNow;
}
