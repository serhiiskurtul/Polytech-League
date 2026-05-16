using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Basketball;

namespace PolytechLeague.Application.Dto.BasketballDto;

public class BasketballTeamRequest
{
    public required string Name { get; set; }
    public long EventId { get; set; }
    public ICollection<long>? PlayerIds { get; set; }
}

public class BasketballTeamResponse
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public ICollection<PlayerDto>? Players { get; set; }
}

public static class BasketballTeamDtoExtensions
{
    public static BasketballTeam FromRequest(this BasketballTeamRequest request)
    {
        return new BasketballTeam
        {
            Name = request.Name,
            EventId = request.EventId,
            PlayerIds = request.PlayerIds
        };
    }

    public static BasketballTeamResponse ToResponse(this BasketballTeam entity)
    {
        return new BasketballTeamResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Players = entity.Players?.Select(p => p.ToDto()).ToList()
        };
    }
}
