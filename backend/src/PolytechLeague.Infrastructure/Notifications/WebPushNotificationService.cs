using Microsoft.Extensions.Options;
using PolytechLeague.Application.Configuration;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Persistence.Database;
using WebPush;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace PolytechLeague.Infrastructure.Notifications;

public class WebPushNotificationService(
    IOptions<VapidSettings> _vapidOptions,
    ApplicationDbContext _context,
    ILogger<WebPushNotificationService> _logger) : IPushNotificationService
{
    public async Task SendPushNotificationAsync(long userId, string title, string message, string? url = null)
    {
        var subscriptions = await _context.PushSubscriptions
            .Where(s => s.UserId == userId)
            .ToListAsync();

        if (subscriptions.Count == 0) return;

        var vapidDetails = new VapidDetails(
            _vapidOptions.Value.Subject,
            _vapidOptions.Value.PublicKey,
            _vapidOptions.Value.PrivateKey);

        var webPushClient = new WebPushClient();
        
        var payload = JsonSerializer.Serialize(new
        {
            title,
            body = message,
            url = url ?? "/"
        });

        foreach (var sub in subscriptions)
        {
            try
            {
                var pushSubscription = new PushSubscription(sub.Endpoint, sub.P256DH, sub.Auth);
                await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
            }
            catch (WebPushException ex)
            {
                if (ex.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.Gone || 
                    ex.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Subscription is no longer valid
                    _context.PushSubscriptions.Remove(sub);
                }
                _logger.LogError(ex, "Error sending push notification to endpoint {Endpoint}", sub.Endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending push notification");
            }
        }

        await _context.SaveChangesAsync();
    }
}
