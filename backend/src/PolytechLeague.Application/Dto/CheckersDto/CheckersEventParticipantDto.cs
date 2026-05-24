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
    public int? BoardRank { get; set; }
    public string? GroupName { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RegistrationStatus Status { get; set; }
}

public static class CheckersEventParticipantDtoExtensions
{
    public static CheckersEventParticipantDto ToDto(this CheckersEventParticipant participant)
    {
        string? firstName = participant.PlayerFirstName ?? participant.Player?.FirstName;
        string? lastName = participant.PlayerLastName ?? participant.Player?.LastName;
        string? fullName = (firstName != null || lastName != null) 
            ? $"{firstName} {lastName}".Trim() 
            : null;

        return new CheckersEventParticipantDto
        {
            Id = participant.Id,
            UserId = participant.Player?.UserId,
            TournamentId = participant.TournamentId,
            PlayerId = participant.PlayerId,
            PlayerName = fullName,
            FirstName = firstName,
            LastName = lastName,
            Points = participant.Points,
            Rating = participant.StartingRating,
            Rank = participant.Rank,
            BoardRank = participant.BoardRank,
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
            BoardRank = dto.BoardRank,
            GroupName = dto.GroupName,
            PlayerFirstName = dto.FirstName,
            PlayerLastName = dto.LastName,
            Status = dto.Status
        };
    }
}
