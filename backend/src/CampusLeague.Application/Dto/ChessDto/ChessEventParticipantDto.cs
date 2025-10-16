using CampusLeague.Domain.Entities.Chess;

namespace CampusLeague.Application.Dto.ChessDto;

public class ChessEventParticipantDto
{
    public long PlayerId { get; set; }
    public double Points { get; set; }
    public int Rank { get; set; }
}

public static class ChessEventParticipantDtoExtensions
{
    public static ChessEventParticipantDto ToDto(this ChessEventParticipant participant)
    {
        return new ChessEventParticipantDto
        {
            PlayerId = participant.PlayerId,
            Points = participant.Points,
            Rank = participant.Rank
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