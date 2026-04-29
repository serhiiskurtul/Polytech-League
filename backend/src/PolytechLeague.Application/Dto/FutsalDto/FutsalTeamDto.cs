using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Futsal;

namespace PolytechLeague.Application.Dto.FutsalDto;

public class FutsalTeamRequest
{
    public required string Name { get; set; }
    public long EventId { get; set; }
    public ICollection<long>? PlayerIds { get; set; }
    public long? CaptainId { get; set; }
    public int Points { get; set; }
}

public class FutsalTeamResponse
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public ICollection<PlayerDto>? Players { get; set; }
    public long? CaptainId { get; set; }
    public int Points { get; set; }
}

public static class FutsalTeamDtoExtensions
{
    public static FutsalTeam FromRequest(this FutsalTeamRequest request)
    {
        return new FutsalTeam
        {
            Name = request.Name,
            EventId = request.EventId,
            PlayerIds = request.PlayerIds,
            CaptainId = request.CaptainId,
            Points = request.Points
        };
    }

    public static FutsalTeamResponse ToResponse(this FutsalTeam entity)
    {
        return new FutsalTeamResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Players = entity.Players?.Select(p => p.ToDto()).ToList(),
            CaptainId = entity.CaptainId,
            Points = entity.Points
        };
    }
}
