using PolytechLeague.Domain.Entities.Armwrestling;
using PolytechLeague.Domain.Entities.Shared;
using System.Text.Json.Serialization;

namespace PolytechLeague.Application.Dto.ArmwrestlingDto;

public class ArmwrestlingEventParticipantDto
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public long TournamentId { get; set; }
    public long PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public double Weight { get; set; }
    public int Rank { get; set; }
    public string? GroupName { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RegistrationStatus Status { get; set; }
}

public static class ArmwrestlingEventParticipantDtoExtensions
{
    public static ArmwrestlingEventParticipantDto ToDto(this ArmwrestlingEventParticipant participant)
    {
        string? firstName = participant.PlayerFirstName ?? participant.Player?.FirstName;
        string? lastName = participant.PlayerLastName ?? participant.Player?.LastName;
        string? fullName = (firstName != null || lastName != null) 
            ? $"{firstName} {lastName}".Trim() 
            : null;

        return new ArmwrestlingEventParticipantDto
        {
            Id = participant.PlayerId,
            UserId = participant.Player?.UserId,
            TournamentId = participant.TournamentId,
            PlayerId = participant.PlayerId,
            PlayerName = fullName,
            FirstName = firstName,
            LastName = lastName,
            Weight = participant.Weight,
            Rank = participant.Rank,
            GroupName = participant.GroupName ?? participant.Player?.Group,
            Status = participant.Status
        };
    }

    public static ArmwrestlingEventParticipant FromDto(this ArmwrestlingEventParticipantDto dto, long tournamentId)
    {
        return new ArmwrestlingEventParticipant
        {
            TournamentId = tournamentId,
            PlayerId = dto.PlayerId,
            Weight = dto.Weight,
            Rank = dto.Rank,
            GroupName = dto.GroupName,
            PlayerFirstName = dto.FirstName,
            PlayerLastName = dto.LastName,
            Status = dto.Status
        };
    }
}
