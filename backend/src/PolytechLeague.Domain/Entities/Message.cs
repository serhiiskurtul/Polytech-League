using System;

namespace PolytechLeague.Domain.Entities;

public class Message
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public virtual Conversation Conversation { get; set; } = null!;

    public long SenderId { get; set; }
    public virtual User Sender { get; set; } = null!;

    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
}
