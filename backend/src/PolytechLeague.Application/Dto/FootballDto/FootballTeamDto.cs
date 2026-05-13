using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Football;

namespace PolytechLeague.Application.Dto.FootballDto;

public class FootballTeamRequest
{
    public required string Name { get; set; }
    public long EventId { get; set; }
    public ICollection<long>? PlayerIds { get; set; }
    public long? CaptainId { get; set; }
    public long? GoalkeeperId { get; set; }
    public int Points { get; set; }
}

public class FootballTeamResponse
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public ICollection<PlayerDto>? Players { get; set; }
    public long? CaptainId { get; set; }
    public long? GoalkeeperId { get; set; }
    public int Points { get; set; }
}

public static class FootballTeamDtoExtensions
{
    public static FootballTeam FromRequest(this FootballTeamRequest request)
    {
        return new FootballTeam
        {
            Name = request.Name,
            EventId = request.EventId,
            PlayerIds = request.PlayerIds,
            CaptainId = request.CaptainId,
            GoalkeeperId = request.GoalkeeperId,
            Points = request.Points
        };
    }

    public static FootballTeamResponse ToResponse(this FootballTeam entity)
    {
        return new FootballTeamResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Players = entity.Players?.Select(p => p.ToDto()).ToList(),
            CaptainId = entity.CaptainId,
            GoalkeeperId = entity.GoalkeeperId,
            Points = entity.Points
        };
    }
}
