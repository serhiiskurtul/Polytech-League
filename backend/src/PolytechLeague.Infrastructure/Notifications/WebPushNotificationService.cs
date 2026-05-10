using Microsoft.Extensions.Options;
using PolytechLeague.Application.Configuration;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Persistence.Database;
using WebPush;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using EntityPushSubscription = PolytechLeague.Domain.Entities.PushSubscription;

namespace PolytechLeague.Infrastructure.Notifications;

public class WebPushNotificationService(
    IOptions<VapidSettings> _vapidOptions,
    ApplicationDbContext _context,
    ILogger<WebPushNotificationService> _logger) : IPushNotificationService
{
    public async Task SendPushNotificationAsync(long userId, string title, string message, string? url = null, string? tag = null)
    {
        var subscriptions = await _context.PushSubscriptions
            .Where(s => s.UserId == userId)
            .ToListAsync();

        if (subscriptions.Count == 0) return;

        await SendToSubscriptionsAsync(subscriptions, title, message, url, tag);
    }

    public async Task SendPushNotificationToAllAsync(string title, string message, string? url = null, bool excludeAdmins = false, string? tag = null)
    {
        var query = _context.PushSubscriptions
            .Include(s => s.User)
            .AsQueryable();

        if (excludeAdmins)
        {
            query = query.Where(s => s.User!.Role != Domain.Entities.UserRole.Admin);
        }

        var subscriptions = await query.ToListAsync();
        if (subscriptions.Count == 0) return;

        await SendToSubscriptionsAsync(subscriptions, title, message, url, tag);
    }

    private async Task SendToSubscriptionsAsync(List<EntityPushSubscription> subscriptions, string title, string message, string? url = null, string? tag = null)
    {
        var vapidDetails = new VapidDetails(
            _vapidOptions.Value.Subject,
            _vapidOptions.Value.PublicKey,
            _vapidOptions.Value.PrivateKey);

        var webPushClient = new WebPushClient();
        
        var payload = JsonSerializer.Serialize(new
        {
            title,
            body = message,
            url = url ?? "/",
            tag = tag ?? "general"
        });

        foreach (var sub in subscriptions)
        {
            try
            {
                var pushSubscription = new WebPush.PushSubscription(sub.Endpoint, sub.P256DH, sub.Auth);
                await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
            }
            catch (WebPushException ex)
            {
                var statusCode = ex.HttpResponseMessage.StatusCode;
                if (statusCode == System.Net.HttpStatusCode.Gone || 
                    statusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _context.PushSubscriptions.Remove(sub);
                    _logger.LogWarning("Push subscription expired or not found. Removing from database: {Endpoint}", sub.Endpoint);
                }
                else
                {
                    _logger.LogError(ex, "Error sending push notification. Status code: {StatusCode}. Endpoint: {Endpoint}", statusCode, sub.Endpoint);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending push notification");
            }
        }

        await _context.SaveChangesAsync();
    }
}
