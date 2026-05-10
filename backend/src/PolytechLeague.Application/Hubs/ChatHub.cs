using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PolytechLeague.Application.Services.Interfaces;

namespace PolytechLeague.Application.Hubs;

[Authorize]
public class ChatHub(IChatService chatService) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }
        await base.OnConnectedAsync();
    }

    public async Task SendMessage(long recipientId, string content)
    {
        var senderIdStr = Context.UserIdentifier;
        if (string.IsNullOrEmpty(senderIdStr) || !long.TryParse(senderIdStr, out long senderId))
        {
            return;
        }

        var messageDto = await chatService.SendMessageAsync(senderId, recipientId, content);

        // Send to recipient
        await Clients.Group($"User_{recipientId}").SendAsync("ReceiveMessage", messageDto);
        
        // Send back to sender (to confirm and sync across devices)
        await Clients.Group($"User_{senderId}").SendAsync("ReceiveMessage", messageDto);
    }

    public async Task MarkAsRead(long conversationId)
    {
        var userIdStr = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userIdStr) || !long.TryParse(userIdStr, out long userId))
        {
            return;
        }

        await chatService.MarkAsReadAsync(conversationId, userId);
        
        // Optionally notify the other user that messages were read
    }
}
