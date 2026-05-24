using PolytechLeague.Domain.Entities.Chess;
using PolytechLeague.Domain.Entities.Shared;
using System.Text.Json.Serialization;

namespace PolytechLeague.Application.Dto.ChessDto;

public class ChessEventParticipantDto
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public long ChessEventId { get; set; }
    public long PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int Rating { get; set; }
    public double Points { get; set; }
    public int Rank { get; set; }
    public int? BoardRank { get; set; }
    public double Buchholz { get; set; }
    public double SonnebornBerger { get; set; }
    public string? FideId { get; set; }
    public string? Title { get; set; }
    public string? GroupName { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RegistrationStatus Status { get; set; }
    public bool IsGuest { get; set; }
}

public static class ChessEventParticipantDtoExtensions
{
    public static ChessEventParticipantDto ToDto(this ChessEventParticipant participant)
    {
        string? firstName = participant.Player?.FirstName ?? participant.PlayerFirstName;
        string? lastName = participant.Player?.LastName ?? participant.PlayerLastName;
        string? fullName = (firstName != null || lastName != null) 
            ? $"{firstName} {lastName}".Trim() 
            : null;

        return new ChessEventParticipantDto
        {
            Id = participant.PlayerId, // Using PlayerId as unique id for frontend key
            UserId = participant.Player?.UserId,
            ChessEventId = participant.TournamentId,
            PlayerId = participant.PlayerId,
            PlayerName = fullName,
            FirstName = firstName,
            LastName = lastName,
            Rating = participant.Rating != 0 ? participant.Rating : (participant.Player?.ChessRating ?? 0),
            Points = participant.Points,
            Rank = participant.Rank,
            BoardRank = participant.BoardRank,
            Buchholz = 0,
            SonnebornBerger = 0,
            FideId = participant.Player?.ChessFideId,
            Title = participant.Player?.ChessTitle,
            GroupName = participant.GroupName ?? participant.Player?.Group,
            Status = participant.Status,
            IsGuest = participant.Player?.IsGuest ?? false
        };
    }

    public static ChessEventParticipant FromDto(this ChessEventParticipantDto dto, long tournamentId)
    {
        return new ChessEventParticipant
        {
            TournamentId = tournamentId,
            PlayerId = dto.PlayerId,
            Points = dto.Points,
            Rank = dto.Rank,
            BoardRank = dto.BoardRank,
            GroupName = dto.GroupName,
            PlayerFirstName = dto.FirstName,
            PlayerLastName = dto.LastName,
            Rating = dto.Rating,
            Status = dto.Status
        };
    }
}