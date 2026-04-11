using PolytechLeague.Domain.Entities.Chess;

namespace PolytechLeague.Application.Dto.ChessDto;

public class ChessEventParticipantDto
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public long ChessEventId { get; set; }
    public long PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public int Rating { get; set; }
    public double Points { get; set; }
    public int Rank { get; set; }
    public double Buchholz { get; set; }
    public double SonnebornBerger { get; set; }
    public string? FideId { get; set; }
    public string? Title { get; set; }
}

public static class ChessEventParticipantDtoExtensions
{
    public static ChessEventParticipantDto ToDto(this ChessEventParticipant participant)
    {
        return new ChessEventParticipantDto
        {
            Id = participant.PlayerId, // Using PlayerId as unique id for frontend key
            UserId = participant.Player?.UserId,
            ChessEventId = participant.TournamentId,
            PlayerId = participant.PlayerId,
            PlayerName = participant.Player != null ? $"{participant.Player.FirstName} {participant.Player.LastName}" : null,
            Rating = participant.Player?.RatingELO ?? 0,
            Points = participant.Points,
            Rank = participant.Rank,
            Buchholz = 0,
            SonnebornBerger = 0,
            FideId = participant.Player?.FideId,
            Title = participant.Player?.Title
        };
    }

    public static ChessEventParticipant FromDto(this ChessEventParticipantDto dto, long tournamentId)
    {
        return new ChessEventParticipant
        {
            TournamentId = tournamentId,
            PlayerId = dto.PlayerId,
            Points = dto.Points,
            Rank = dto.Rank
        };
    }
}