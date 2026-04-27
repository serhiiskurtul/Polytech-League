using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Application.Dto.GeneralDto;

public class ProfileMergeRequestDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string? UserFullName { get; set; }
    public long TargetPlayerId { get; set; }
    public string? TargetPlayerName { get; set; }
    public string? TargetPlayerGroup { get; set; }
    public RegistrationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AdminComment { get; set; }
}
