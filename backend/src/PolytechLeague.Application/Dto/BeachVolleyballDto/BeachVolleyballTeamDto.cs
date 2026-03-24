using PolytechLeague.Domain.Entities.BeachVolleyball;

namespace PolytechLeague.Application.Dto.BeachVolleyballDto;

public class BeachVolleyballTeamRequest
{
    public required string Name { get; set; }
}

public class BeachVolleyballTeamResponse
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public ICollection<BeachVolleyballPlayerResponse>? Players { get; set; }
}

public static class BeachVolleyballTeamDtoExtensions
{
    public static BeachVolleyballTeam FromRequest(this BeachVolleyballTeamRequest request)
    {
        return new BeachVolleyballTeam
        {
            Name = request.Name
        };
    }

    public static BeachVolleyballTeamResponse ToResponse(this BeachVolleyballTeam entity)
    {
        return new BeachVolleyballTeamResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Players = entity.Players?.Select(p => p.ToResponse()).ToList()
        };
    }
}
