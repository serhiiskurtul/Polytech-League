using System;
using System.Collections.Generic;

namespace PolytechLeague.Domain.Entities;

public class Conversation
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

    public long User1Id { get; set; }
    public virtual User User1 { get; set; } = null!;

    public long User2Id { get; set; }
    public virtual User User2 { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
