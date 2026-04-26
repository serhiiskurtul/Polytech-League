using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Domain.Entities.Shared;
using System.Text.Json.Serialization;

namespace PolytechLeague.Application.Dto.CheckersDto;

public class CheckersEventParticipantDto
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public long TournamentId { get; set; }
    public long PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public double Points { get; set; }
    public int Rating { get; set; }
    public int Rank { get; set; }
    public string? GroupName { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RegistrationStatus Status { get; set; }
}

public static class CheckersEventParticipantDtoExtensions
{
    public static CheckersEventParticipantDto ToDto(this CheckersEventParticipant participant)
    {
        return new CheckersEventParticipantDto
        {
            Id = participant.Id,
            UserId = participant.Player?.UserId,
            TournamentId = participant.TournamentId,
            PlayerId = participant.PlayerId,
            PlayerName = participant.Player != null ? $"{participant.Player.FirstName} {participant.Player.Patronymic ?? ""} {participant.Player.LastName}".Replace("  ", " ").Trim() : null,
            FirstName = participant.Player?.FirstName,
            LastName = participant.Player?.LastName,
            Points = participant.Points,
            Rating = participant.StartingRating,
            Rank = participant.Rank,
            GroupName = participant.GroupName ?? participant.Player?.Group,
            Status = participant.Status
        };
    }

    public static CheckersEventParticipant FromDto(this CheckersEventParticipantDto dto, long tournamentId)
    {
        return new CheckersEventParticipant
        {
            TournamentId = tournamentId,
            PlayerId = dto.PlayerId,
            StartingRating = dto.Rating,
            Points = dto.Points,
            Rank = dto.Rank,
            GroupName = dto.GroupName,
            Status = dto.Status
        };
    }
}
