using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations;

public class NotificationService(
    ApplicationDbContext _context,
    IPushNotificationService _pushNotificationService) : INotificationService
{
    public async Task SendNotificationToAllAsync(string title, string message, NotificationType type, string? link = null)
    {
        var users = await _context.Users
            .Where(u => !u.IsDeleted && u.Role != UserRole.Admin)
            .ToListAsync();

        foreach (var user in users)
        {
            var notification = new Notification
            {
                UserId = user.Id,
                Title = title,
                Message = message,
                Type = type,
                Link = link,
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);
            
            // Send push notification asynchronously
            _ = _pushNotificationService.SendPushNotificationAsync(user.Id, title, message, link);
        }

        await _context.SaveChangesAsync();
    }

    public async Task SendNotificationToAdminsAsync(string title, string message, NotificationType type, string? link = null)
    {
        var admins = await _context.Users
            .Where(u => !u.IsDeleted && u.Role == UserRole.Admin)
            .ToListAsync();

        foreach (var admin in admins)
        {
            var notification = new Notification
            {
                UserId = admin.Id,
                Title = title,
                Message = message,
                Type = type,
                Link = link,
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);
            
            // Send push notification asynchronously
            _ = _pushNotificationService.SendPushNotificationAsync(admin.Id, title, message, link);
        }

        await _context.SaveChangesAsync();
    }

    public async Task SendNotificationToUserAsync(long userId, string title, string message, NotificationType type, string? link = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            Link = link,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        
        // Send actual push notification
        await _pushNotificationService.SendPushNotificationAsync(userId, title, message, link);
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(long userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notifications.Select(n => n.ToDto());
    }

    public async Task MarkAsReadAsync(long notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(long userId)
    {
        var unread = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in unread)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteNotificationAsync(long notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearUserNotificationsAsync(long userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && n.IsRead)
            .ToListAsync();

        _context.Notifications.RemoveRange(notifications);
        await _context.SaveChangesAsync();
    }
}
