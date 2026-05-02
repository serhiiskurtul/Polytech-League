using PolytechLeague.Domain.Entities.Badminton;
using PolytechLeague.Domain.Entities.Shared;
using System.Text.Json.Serialization;

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
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RegistrationStatus Status { get; set; }
}

public static class BadmintonEventParticipantDtoExtensions
{
    public static BadmintonEventParticipantDto ToDto(this BadmintonEventParticipant participant)
    {
        string? firstName = participant.PlayerFirstName ?? participant.Player?.FirstName;
        string? lastName = participant.PlayerLastName ?? participant.Player?.LastName;
        string? fullName = (firstName != null || lastName != null) 
            ? $"{firstName} {lastName}".Trim() 
            : null;

        return new BadmintonEventParticipantDto
        {
            Id = participant.PlayerId,
            UserId = participant.Player?.UserId,
            EventId = participant.EventId,
            PlayerId = participant.PlayerId,
            PlayerName = fullName,
            FirstName = firstName,
            LastName = lastName,
            Points = participant.Points,
            Rank = participant.Rank,
            GroupName = participant.GroupName ?? participant.Player?.Group,
            Status = participant.Status
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
            GroupName = dto.GroupName,
            PlayerFirstName = dto.FirstName,
            PlayerLastName = dto.LastName,
            Status = dto.Status
        };
    }
}
