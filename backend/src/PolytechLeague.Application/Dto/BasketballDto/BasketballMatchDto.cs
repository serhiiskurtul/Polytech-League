using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Basketball;

namespace PolytechLeague.Application.Dto.BasketballDto;

public class BasketballMatchRequest
{
    public long EventId { get; set; }
    public long HomeTeamId { get; set; }
    public long AwayTeamId { get; set; }
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
    public string? Stage { get; set; }
    public long? WinnerTeamId { get; set; }
    public ICollection<long>? HomeRosterIds { get; set; }
    public ICollection<long>? AwayRosterIds { get; set; }
}

public class BasketballMatchResponse
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public BasketballEventResponse? Event { get; set; }
    
    public long HomeTeamId { get; set; }
    public BasketballTeamResponse? HomeTeam { get; set; }
    
    public long AwayTeamId { get; set; }
    public BasketballTeamResponse? AwayTeam { get; set; }
    
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
    
    public long? WinnerTeamId { get; set; }
    public BasketballTeamResponse? WinnerTeam { get; set; }
    
    public string? Stage { get; set; }
    
    public ICollection<PlayerDto>? HomeRoster { get; set; }
    public ICollection<PlayerDto>? AwayRoster { get; set; }
}

public static class BasketballMatchDtoExtensions
{
    public static BasketballMatch FromRequest(this BasketballMatchRequest request)
    {
        return new BasketballMatch
        {
            EventId = request.EventId,
            HomeTeamId = request.HomeTeamId,
            AwayTeamId = request.AwayTeamId,
            HomeTeamScore = request.HomeTeamScore,
            AwayTeamScore = request.AwayTeamScore,
            WinnerTeamId = request.WinnerTeamId,
            Stage = request.Stage,
            HomeRosterIds = request.HomeRosterIds,
            AwayRosterIds = request.AwayRosterIds
        };
    }

    public static BasketballMatchResponse ToResponse(this BasketballMatch entity)
    {
        return new BasketballMatchResponse
        {
            Id = entity.Id,
            EventId = entity.EventId,
            HomeTeamId = entity.HomeTeamId,
            HomeTeam = entity.HomeTeam?.ToResponse(),
            AwayTeamId = entity.AwayTeamId,
            AwayTeam = entity.AwayTeam?.ToResponse(),
            HomeTeamScore = entity.HomeTeamScore,
            AwayTeamScore = entity.AwayTeamScore,
            WinnerTeamId = entity.WinnerTeamId,
            WinnerTeam = entity.WinnerTeam?.ToResponse(),
            Stage = entity.Stage,
            HomeRoster = entity.HomeRoster?.Select(p => p.ToDto()).ToList(),
            AwayRoster = entity.AwayRoster?.Select(p => p.ToDto()).ToList()
        };
    }
}
