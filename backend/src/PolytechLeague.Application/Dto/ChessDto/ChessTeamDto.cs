using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Chess;

namespace PolytechLeague.Application.Dto.ChessDto;

public class ChessTeamRequest
{
    public required string Name { get; set; }
    public long EventId { get; set; }
    public ICollection<long>? PlayerIds { get; set; }
}

public class ChessTeamResponse
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public ICollection<PlayerDto>? Players { get; set; }
}

public static class ChessTeamDtoExtensions
{
    public static ChessTeam FromRequest(this ChessTeamRequest request)
    {
        return new ChessTeam
        {
            Name = request.Name,
            EventId = request.EventId,
            PlayerIds = request.PlayerIds
        };
    }

    public static ChessTeamResponse ToResponse(this ChessTeam entity)
    {
        return new ChessTeamResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Players = entity.Players?.Select(p => p.ToDto()).ToList()
        };
    }
}
