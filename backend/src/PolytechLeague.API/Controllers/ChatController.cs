using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Dto.Chat;
using PolytechLeague.Application.Services.Interfaces;

namespace PolytechLeague.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatController(IChatService chatService) : ControllerBase
{
    [HttpGet("conversations")]
    public async Task<ActionResult<IEnumerable<ConversationDto>>> GetConversations()
    {
        var userId = GetUserId();
        if (userId == -1) return Unauthorized();

        var conversations = await chatService.GetConversationsAsync(userId);
        return Ok(conversations);
    }

    [HttpGet("conversations/{conversationId}/messages")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(long conversationId, [FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        var messages = await chatService.GetConversationMessagesAsync(conversationId, skip, take);
        return Ok(messages);
    }

    [HttpGet("conversations/with-user/{otherUserId}")]
    public async Task<ActionResult<long>> GetConversationIdWithUser(long otherUserId)
    {
        var userId = GetUserId();
        if (userId == -1) return Unauthorized();

        var conversationId = await chatService.GetOrCreateConversationIdAsync(userId, otherUserId);
        return Ok(conversationId);
    }

    [HttpPost("mark-read/{conversationId}")]
    public async Task<IActionResult> MarkAsRead(long conversationId)
    {
        var userId = GetUserId();
        if (userId == -1) return Unauthorized();

        await chatService.MarkAsReadAsync(conversationId, userId);
        return Ok();
    }

    private long GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (long.TryParse(userIdStr, out long userId))
        {
            return userId;
        }
        return -1;
    }
}
