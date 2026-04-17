using System;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Dto;

public class NotificationDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Link { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public static class NotificationExtensions
{
    public static NotificationDto ToDto(this Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            Link = notification.Link,
            Type = notification.Type.ToString().ToLower(),
            Icon = GetIcon(notification.Type),
            Color = GetColor(notification.Type)
        };
    }

    private static string GetIcon(NotificationType type)
    {
        return type switch
        {
            NotificationType.Tournament => "fas fa-trophy",
            NotificationType.Match => "fas fa-futbol",
            NotificationType.Achievement => "fas fa-award",
            NotificationType.Team => "fas fa-users",
            _ => "fas fa-bell"
        };
    }

    private static string GetColor(NotificationType type)
    {
        return type switch
        {
            NotificationType.Tournament => "#00c853", // success
            NotificationType.Match => "#2196f3",      // primary
            NotificationType.Achievement => "#ffc107", // warning
            NotificationType.Team => "#e91e63",       // accent
            _ => "#9e9e9e"                           // muted
        };
    }
}
