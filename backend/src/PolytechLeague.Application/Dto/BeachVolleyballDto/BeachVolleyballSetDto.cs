using PolytechLeague.Domain.Entities.BeachVolleyball;

namespace PolytechLeague.Application.Dto.BeachVolleyballDto;

public class BeachVolleyballSetRequest
{
    public long MatchId { get; set; }
    public int SetNumber { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
}

public class BeachVolleyballSetResponse
{
    public long Id { get; set; }
    public long MatchId { get; set; }
    public int SetNumber { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
}

public static class BeachVolleyballSetDtoExtensions
{
    public static BeachVolleyballSet FromRequest(this BeachVolleyballSetRequest request)
    {
        return new BeachVolleyballSet
        {
            MatchId = request.MatchId,
            SetNumber = request.SetNumber,
            HomeScore = request.HomeScore,
            AwayScore = request.AwayScore
        };
    }

    public static BeachVolleyballSetResponse ToResponse(this BeachVolleyballSet entity)
    {
        return new BeachVolleyballSetResponse
        {
            Id = entity.Id,
            MatchId = entity.MatchId,
            SetNumber = entity.SetNumber,
            HomeScore = entity.HomeScore,
            AwayScore = entity.AwayScore
        };
    }
}
