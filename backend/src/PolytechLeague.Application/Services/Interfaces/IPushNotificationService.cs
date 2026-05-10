using System.Threading.Tasks;

namespace PolytechLeague.Application.Services.Interfaces;

public interface IPushNotificationService
{
    Task SendPushNotificationAsync(long userId, string title, string message, string? url = null, string? tag = null);
    Task SendPushNotificationToAllAsync(string title, string message, string? url = null, bool excludeAdmins = false, string? tag = null);
}
