using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Futsal;

namespace PolytechLeague.Application.Dto.FutsalDto;

public class FutsalMatchRequest
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
    public ICollection<FutsalGoalRequest>? Goals { get; set; }
}

public class FutsalMatchResponse
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public FutsalEventResponse? Event { get; set; }
    
    public long HomeTeamId { get; set; }
    public FutsalTeamResponse? HomeTeam { get; set; }
    
    public long AwayTeamId { get; set; }
    public FutsalTeamResponse? AwayTeam { get; set; }
    
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
    
    public long? WinnerTeamId { get; set; }
    public FutsalTeamResponse? WinnerTeam { get; set; }
    
    public string? Stage { get; set; }
    
    public ICollection<PlayerDto>? HomeRoster { get; set; }
    public ICollection<PlayerDto>? AwayRoster { get; set; }
    public long? HomeGoalkeeperId { get; set; }
    public PlayerDto? HomeGoalkeeper { get; set; }
    public long? AwayGoalkeeperId { get; set; }
    public PlayerDto? AwayGoalkeeper { get; set; }
    public ICollection<FutsalGoalResponse>? Goals { get; set; }
}

public static class FutsalMatchDtoExtensions
{
    public static FutsalMatch FromRequest(this FutsalMatchRequest request)
    {
        return new FutsalMatch
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
            Goals = request.Goals?.Select(g => g.FromRequest(0)).ToList() // MatchId will be set in service
        };
    }

    public static FutsalMatchResponse ToResponse(this FutsalMatch entity)
    {
        return new FutsalMatchResponse
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
            Goals = entity.Goals?.Select(g => g.ToResponse()).ToList()
        };
    }
}
