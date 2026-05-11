using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Dto.Chat;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations;

public class ChatService(
    ApplicationDbContext _context,
    INotificationService _notificationService) : IChatService
{
    public async Task<IEnumerable<ConversationDto>> GetConversationsAsync(long userId)
    {
        var conversations = await _context.Conversations
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .OrderByDescending(c => c.LastMessageAt)
            .Select(c => new ConversationDto
            {
                Id = c.Id,
                OtherUserId = c.User1Id == userId ? c.User2Id : c.User1Id,
                OtherUserName = c.User1Id == userId 
                    ? c.User2.FirstName + " " + c.User2.LastName 
                    : c.User1.FirstName + " " + c.User1.LastName,
                OtherUserPhoto = null, // Can be added if photo field exists in User
                LastMessageAt = c.LastMessageAt,
                UnreadCount = c.Messages.Count(m => m.SenderId != userId && !m.IsRead),
                LastMessage = c.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => new MessageDto
                    {
                        Id = m.Id,
                        Content = m.Content,
                        SentAt = m.SentAt,
                        SenderId = m.SenderId,
                        IsRead = m.IsRead
                    })
                    .FirstOrDefault()
            })
            .ToListAsync();

        return conversations;
    }

    public async Task<IEnumerable<MessageDto>> GetConversationMessagesAsync(long conversationId, int skip = 0, int take = 50)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.SentAt)
            .Skip(skip)
            .Take(take)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                SenderId = m.SenderId,
                SenderName = m.Sender.FirstName + " " + m.Sender.LastName,
                Content = m.Content,
                SentAt = m.SentAt,
                IsRead = m.IsRead
            })
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task<MessageDto> SendMessageAsync(long senderId, long recipientId, string content)
    {
        var conversationId = await GetOrCreateConversationIdAsync(senderId, recipientId);
        
        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        
        var conversation = await _context.Conversations.FindAsync(conversationId);
        if (conversation != null)
        {
            conversation.LastMessageAt = message.SentAt;
        }

        await _context.SaveChangesAsync();

        var sender = await _context.Users.FindAsync(senderId);

        // Send push notification to recipient
        await _notificationService.SendNotificationToUserAsync(
            recipientId,
            $"Нове повідомлення від {sender?.FirstName} {sender?.LastName}",
            content.Length > 50 ? content.Substring(0, 47) + "..." : content,
            NotificationType.Message,
            $"/notifications?tab=messages&userId={senderId}",
            $"chat_msg_{senderId}"
        );

        return new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = senderId,
            SenderName = sender != null ? sender.FirstName + " " + sender.LastName : "Unknown",
            Content = message.Content,
            SentAt = message.SentAt,
            IsRead = message.IsRead
        };
    }

    public async Task MarkAsReadAsync(long conversationId, long userId)
    {
        var messages = await _context.Messages
            .Where(m => m.ConversationId == conversationId && m.SenderId != userId && !m.IsRead)
            .ToListAsync();

        foreach (var message in messages)
        {
            message.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<long> GetOrCreateConversationIdAsync(long user1Id, long user2Id)
    {
        var firstId = Math.Min(user1Id, user2Id);
        var secondId = Math.Max(user1Id, user2Id);

        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.User1Id == firstId && c.User2Id == secondId);

        if (conversation != null)
            return conversation.Id;

        conversation = new Conversation
        {
            User1Id = firstId,
            User2Id = secondId,
            CreatedAt = DateTime.UtcNow,
            LastMessageAt = DateTime.UtcNow
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        return conversation.Id;
    }
}
