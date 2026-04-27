using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Domain.Entities;

public class ProfileMergeRequest
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public User? User { get; set; }
    public long TargetPlayerId { get; set; }
    public Player? TargetPlayer { get; set; }
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? AdminComment { get; set; }
}
