using PolytechLeague.Domain.Entities.Volleyball;

namespace PolytechLeague.Application.Dto.VolleyballDto;

public class VolleyballSetRequest
{
    public long MatchId { get; set; }
    public int SetNumber { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
}

public class VolleyballSetResponse
{
    public long Id { get; set; }
    public long MatchId { get; set; }
    public int SetNumber { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
}

public static class VolleyballSetDtoExtensions
{
    public static VolleyballSet FromRequest(this VolleyballSetRequest request)
    {
        return new VolleyballSet
        {
            MatchId = request.MatchId,
            SetNumber = request.SetNumber,
            HomeScore = request.HomeScore,
            AwayScore = request.AwayScore
        };
    }

    public static VolleyballSetResponse ToResponse(this VolleyballSet entity)
    {
        return new VolleyballSetResponse
        {
            Id = entity.Id,
            MatchId = entity.MatchId,
            SetNumber = entity.SetNumber,
            HomeScore = entity.HomeScore,
            AwayScore = entity.AwayScore
        };
    }
}
