using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Basketball3x3;

namespace PolytechLeague.Application.Dto.Basketball3x3Dto;

public class Basketball3x3MatchRequest
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

public class Basketball3x3MatchResponse
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public Basketball3x3EventResponse? Event { get; set; }
    
    public long HomeTeamId { get; set; }
    public Basketball3x3TeamResponse? HomeTeam { get; set; }
    
    public long AwayTeamId { get; set; }
    public Basketball3x3TeamResponse? AwayTeam { get; set; }
    
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
    
    public long? WinnerTeamId { get; set; }
    public Basketball3x3TeamResponse? WinnerTeam { get; set; }
    
    public string? Stage { get; set; }
    
    public ICollection<PlayerDto>? HomeRoster { get; set; }
    public ICollection<PlayerDto>? AwayRoster { get; set; }
}

public static class Basketball3x3MatchDtoExtensions
{
    public static Basketball3x3Match FromRequest(this Basketball3x3MatchRequest request)
    {
        return new Basketball3x3Match
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

    public static Basketball3x3MatchResponse ToResponse(this Basketball3x3Match entity)
    {
        return new Basketball3x3MatchResponse
        {
            Id = entity.Id,
            EventId = entity.EventId,
            Event = entity.Event?.ToResponse(),
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
