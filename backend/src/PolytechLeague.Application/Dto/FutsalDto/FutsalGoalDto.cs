using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Futsal;

namespace PolytechLeague.Application.Dto.FutsalDto;

public class FutsalGoalRequest
{
    public long PlayerId { get; set; }
    public int GoalsCount { get; set; }
}

public class FutsalGoalResponse
{
    public long Id { get; set; }
    public long MatchId { get; set; }
    public long PlayerId { get; set; }
    public PlayerDto? Player { get; set; }
    public int GoalsCount { get; set; }
}

public static class FutsalGoalDtoExtensions
{
    public static FutsalGoal FromRequest(this FutsalGoalRequest request, long matchId)
    {
        return new FutsalGoal
        {
            MatchId = matchId,
            PlayerId = request.PlayerId,
            GoalsCount = request.GoalsCount
        };
    }

    public static FutsalGoalResponse ToResponse(this FutsalGoal entity)
    {
        return new FutsalGoalResponse
        {
            Id = entity.Id,
            MatchId = entity.MatchId,
            PlayerId = entity.PlayerId,
            Player = entity.Player?.ToDto(),
            GoalsCount = entity.GoalsCount
        };
    }
}
