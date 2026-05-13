using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Football;

namespace PolytechLeague.Application.Dto.FootballDto;

public class FootballGoalRequest
{
    public long PlayerId { get; set; }
    public int GoalsCount { get; set; }
}

public class FootballGoalResponse
{
    public long Id { get; set; }
    public long MatchId { get; set; }
    public long PlayerId { get; set; }
    public PlayerDto? Player { get; set; }
    public int GoalsCount { get; set; }
}

public static class FootballGoalDtoExtensions
{
    public static FootballGoal FromRequest(this FootballGoalRequest request, long matchId)
    {
        return new FootballGoal
        {
            MatchId = matchId,
            PlayerId = request.PlayerId,
            GoalsCount = request.GoalsCount
        };
    }

    public static FootballGoalResponse ToResponse(this FootballGoal entity)
    {
        return new FootballGoalResponse
        {
            Id = entity.Id,
            MatchId = entity.MatchId,
            PlayerId = entity.PlayerId,
            Player = entity.Player?.ToDto(),
            GoalsCount = entity.GoalsCount
        };
    }
}
