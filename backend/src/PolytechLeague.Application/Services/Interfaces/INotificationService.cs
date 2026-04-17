using System.Collections.Generic;
using System.Threading.Tasks;
using PolytechLeague.Application.Dto;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Services.Interfaces;

public interface INotificationService
{
    Task SendNotificationToAllAsync(string title, string message, NotificationType type, string? link = null);
    Task SendNotificationToUserAsync(long userId, string title, string message, NotificationType type, string? link = null);
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(long userId);
    Task MarkAsReadAsync(long notificationId);
    Task MarkAllAsReadAsync(long userId);
    Task DeleteNotificationAsync(long notificationId);
    Task ClearUserNotificationsAsync(long userId);
}
