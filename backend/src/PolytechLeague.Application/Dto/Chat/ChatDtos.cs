using System;

namespace PolytechLeague.Application.Dto.Chat;

public class MessageDto
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public long SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}

public class ConversationDto
{
    public long Id { get; set; }
    public long OtherUserId { get; set; }
    public string OtherUserName { get; set; } = string.Empty;
    public string? OtherUserPhoto { get; set; }
    public MessageDto? LastMessage { get; set; }
    public int UnreadCount { get; set; }
    public DateTime LastMessageAt { get; set; }
}

public class SendMessageRequest
{
    public long RecipientId { get; set; }
    public string Content { get; set; } = string.Empty;
}
