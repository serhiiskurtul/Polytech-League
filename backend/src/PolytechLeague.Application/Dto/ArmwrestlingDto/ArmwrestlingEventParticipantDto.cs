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
        return new ArmwrestlingEventParticipantDto
        {
            Id = participant.PlayerId,
            UserId = participant.Player?.UserId,
            TournamentId = participant.TournamentId,
            PlayerId = participant.PlayerId,
            PlayerName = participant.Player != null ? $"{participant.Player.FirstName} {participant.Player.Patronymic ?? ""} {participant.Player.LastName}".Replace("  ", " ").Trim() : null,
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
            Status = dto.Status
        };
    }
}
