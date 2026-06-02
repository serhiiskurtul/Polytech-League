namespace PolytechLeague.Domain.Entities;

public class User
{
    public long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Email { get; set; }
    public bool isEmailConfirmed { get; set; } = false;
    public required string Password { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool IsBlocked { get; set; } = false;
    public string? PhotoUrl { get; set; }
    public DateTime? LastVisitAt { get; set; }

    public ICollection<Notification>? Notifications { get; set; }
}

public enum UserRole
{
    None = -1,
    User = 0,
    Admin = 1,
    Fizorg = 2
}
