using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Volleyball;

namespace PolytechLeague.Application.Dto.VolleyballDto;

public class VolleyballTeamRequest
{
    public required string Name { get; set; }
}

public class VolleyballTeamResponse
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public ICollection<PlayerDto>? Players { get; set; }
}

public static class VolleyballTeamDtoExtensions
{
    public static VolleyballTeam FromRequest(this VolleyballTeamRequest request)
    {
        return new VolleyballTeam
        {
            Name = request.Name
        };
    }

    public static VolleyballTeamResponse ToResponse(this VolleyballTeam entity)
    {
        return new VolleyballTeamResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Players = entity.Players?.Select(p => p.ToDto()).ToList()
        };
    }
}
