using PolytechLeague.Domain.Entities.Checkers;

namespace PolytechLeague.Application.Dto.CheckersDto;

public class CheckersEventParticipantDto
{
    public long UserId { get; set; }
    public int StartingRating { get; set; }
    public double PointsEarned { get; set; }
}

public static class CheckersEventParticipantDtoExtensions
{
    public static CheckersEventParticipantDto ToDto(this CheckersEventParticipant participant)
    {
        return new CheckersEventParticipantDto
        {
            UserId = participant.UserId,
            StartingRating = participant.StartingRating,
            PointsEarned = participant.PointsEarned
        };
    }

    public static CheckersEventParticipant FromDto(this CheckersEventParticipantDto dto, long checkersEventId)
    {
        return new CheckersEventParticipant
        {
            CheckersEventId = checkersEventId,
            UserId = dto.UserId,
            StartingRating = dto.StartingRating,
            PointsEarned = dto.PointsEarned
        };
    }
}
