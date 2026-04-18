using System.Collections.Generic;
using System.Threading.Tasks;
using PolytechLeague.Application.Dto.Chat;

namespace PolytechLeague.Application.Services.Interfaces;

public interface IChatService
{
    Task<IEnumerable<ConversationDto>> GetConversationsAsync(long userId);
    Task<IEnumerable<MessageDto>> GetConversationMessagesAsync(long conversationId, int skip = 0, int take = 50);
    Task<MessageDto> SendMessageAsync(long senderId, long recipientId, string content);
    Task MarkAsReadAsync(long conversationId, long userId);
    Task<long> GetOrCreateConversationIdAsync(long user1Id, long user2Id);
}
