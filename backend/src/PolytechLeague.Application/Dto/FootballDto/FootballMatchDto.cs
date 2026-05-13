using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Football;

namespace PolytechLeague.Application.Dto.FootballDto;

public class FootballMatchRequest
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
    public long? HomeGoalkeeperId { get; set; }
    public long? AwayGoalkeeperId { get; set; }
    public long? HomeCaptainId { get; set; }
    public long? AwayCaptainId { get; set; }
    public ICollection<FootballGoalRequest>? Goals { get; set; }
}

public class FootballMatchResponse
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public FootballEventResponse? Event { get; set; }
    
    public long HomeTeamId { get; set; }
    public FootballTeamResponse? HomeTeam { get; set; }
    
    public long AwayTeamId { get; set; }
    public FootballTeamResponse? AwayTeam { get; set; }
    
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
    
    public long? WinnerTeamId { get; set; }
    public FootballTeamResponse? WinnerTeam { get; set; }
    
    public string? Stage { get; set; }
    
    public ICollection<PlayerDto>? HomeRoster { get; set; }
    public ICollection<PlayerDto>? AwayRoster { get; set; }
    public long? HomeGoalkeeperId { get; set; }
    public PlayerDto? HomeGoalkeeper { get; set; }
    public long? AwayGoalkeeperId { get; set; }
    public PlayerDto? AwayGoalkeeper { get; set; }
    public long? HomeCaptainId { get; set; }
    public PlayerDto? HomeCaptain { get; set; }
    public long? AwayCaptainId { get; set; }
    public PlayerDto? AwayCaptain { get; set; }
    public ICollection<FootballGoalResponse>? Goals { get; set; }
}

public static class FootballMatchDtoExtensions
{
    public static FootballMatch FromRequest(this FootballMatchRequest request)
    {
        return new FootballMatch
        {
            EventId = request.EventId,
            HomeTeamId = request.HomeTeamId,
            AwayTeamId = request.AwayTeamId,
            HomeTeamScore = request.HomeTeamScore,
            AwayTeamScore = request.AwayTeamScore,
            WinnerTeamId = request.WinnerTeamId,
            Stage = request.Stage,
            HomeRosterIds = request.HomeRosterIds,
            AwayRosterIds = request.AwayRosterIds,
            HomeGoalkeeperId = request.HomeGoalkeeperId,
            AwayGoalkeeperId = request.AwayGoalkeeperId,
            HomeCaptainId = request.HomeCaptainId,
            AwayCaptainId = request.AwayCaptainId,
            Goals = request.Goals?.Select(g => g.FromRequest(0)).ToList() // MatchId will be set in service
        };
    }

    public static FootballMatchResponse ToResponse(this FootballMatch entity)
    {
        return new FootballMatchResponse
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
            AwayRoster = entity.AwayRoster?.Select(p => p.ToDto()).ToList(),
            HomeGoalkeeperId = entity.HomeGoalkeeperId,
            HomeGoalkeeper = entity.HomeGoalkeeper?.ToDto(),
            AwayGoalkeeperId = entity.AwayGoalkeeperId,
            AwayGoalkeeper = entity.AwayGoalkeeper?.ToDto(),
            HomeCaptainId = entity.HomeCaptainId,
            HomeCaptain = entity.HomeCaptain?.ToDto(),
            AwayCaptainId = entity.AwayCaptainId,
            AwayCaptain = entity.AwayCaptain?.ToDto(),
            Goals = entity.Goals?.Select(g => g.ToResponse()).ToList()
        };
    }
}
