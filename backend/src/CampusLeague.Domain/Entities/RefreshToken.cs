namespace CampusLeague.Domain.Entities;

public class RefreshToken
{
    public long Id { get; set; }
    public required string Token { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public required long UserId { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
    
}
