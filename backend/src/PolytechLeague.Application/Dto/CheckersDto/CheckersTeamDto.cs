using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Checkers;

namespace PolytechLeague.Application.Dto.CheckersDto;

public class CheckersTeamRequest
{
    public required string Name { get; set; }
    public long EventId { get; set; }
    public ICollection<long>? PlayerIds { get; set; }
    public double Points { get; set; }
    public int Rank { get; set; }
    public string? TournamentGroupName { get; set; }
    public bool IsGeneralTableIncluded { get; set; }
}

public class CheckersTeamResponse
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public ICollection<PlayerDto>? Players { get; set; }
    public double Points { get; set; }
    public int Rank { get; set; }
    public string? TournamentGroupName { get; set; }
    public bool IsGeneralTableIncluded { get; set; }
}

public static class CheckersTeamDtoExtensions
{
    public static CheckersTeam FromRequest(this CheckersTeamRequest request)
    {
        return new CheckersTeam
        {
            Name = request.Name,
            EventId = request.EventId,
            PlayerIds = request.PlayerIds,
            Points = request.Points,
            Rank = request.Rank,
            TournamentGroupName = request.TournamentGroupName,
            IsGeneralTableIncluded = request.IsGeneralTableIncluded
        };
    }

    public static CheckersTeamResponse ToResponse(this CheckersTeam entity)
    {
        return new CheckersTeamResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Players = entity.Players?.Select(p => p.ToDto()).ToList(),
            Points = entity.Points,
            Rank = entity.Rank,
            TournamentGroupName = entity.TournamentGroupName,
            IsGeneralTableIncluded = entity.IsGeneralTableIncluded
        };
    }
}
