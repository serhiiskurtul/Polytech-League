namespace CampusLeague.Domain.Entities;

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
}

public enum UserRole
{
    User = 0,
    Admin = 1
}
