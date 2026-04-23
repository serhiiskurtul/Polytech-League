using PolytechLeague.Domain.Entities.Badminton;

namespace PolytechLeague.Application.Dto.BadmintonDto;

public class BadmintonEventParticipantDto
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public long EventId { get; set; }
    public long PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public double Points { get; set; }
    public int Rank { get; set; }
    public string? GroupName { get; set; }
}

public static class BadmintonEventParticipantDtoExtensions
{
    public static BadmintonEventParticipantDto ToDto(this BadmintonEventParticipant participant)
    {
        return new BadmintonEventParticipantDto
        {
            Id = participant.PlayerId,
            UserId = participant.Player?.UserId,
            EventId = participant.EventId,
            PlayerId = participant.PlayerId,
            PlayerName = participant.Player != null ? $"{participant.Player.FirstName} {participant.Player.Patronymic ?? ""} {participant.Player.LastName}".Replace("  ", " ").Trim() : null,
            Points = participant.Points,
            Rank = participant.Rank,
            GroupName = participant.GroupName ?? participant.Player?.Group
        };
    }

    public static BadmintonEventParticipant FromDto(this BadmintonEventParticipantDto dto, long eventId)
    {
        return new BadmintonEventParticipant
        {
            EventId = eventId,
            PlayerId = dto.PlayerId,
            Points = dto.Points,
            Rank = dto.Rank,
            GroupName = dto.GroupName
        };
    }
}
