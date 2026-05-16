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
    public int? HomePenaltyScore { get; set; }
    public int? AwayPenaltyScore { get; set; }
    public string? Stage { get; set; }
    public long? WinnerTeamId { get; set; }
    public ICollection<long>? HomeRosterIds { get; set; }
    public ICollection<long>? AwayRosterIds { get; set; }
    public long? HomeGoalkeeperId { get; set; }
    public long? AwayGoalkeeperId { get; set; }
    public long? HomeCaptainId { get; set; }
    public long? AwayCaptainId { get; set; }
    public ICollection<FootballGoalRequest>? Goals { get; set; }
    public ICollection<FootballCardRequest>? YellowCards { get; set; }
    public ICollection<FootballCardRequest>? RedCards { get; set; }
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
    public int? HomePenaltyScore { get; set; }
    public int? AwayPenaltyScore { get; set; }
    
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
    public ICollection<FootballCardResponse>? YellowCards { get; set; }
    public ICollection<FootballCardResponse>? RedCards { get; set; }
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
            HomePenaltyScore = request.HomePenaltyScore,
            AwayPenaltyScore = request.AwayPenaltyScore,
            WinnerTeamId = request.WinnerTeamId,
            Stage = request.Stage,
            HomeRosterIds = request.HomeRosterIds,
            AwayRosterIds = request.AwayRosterIds,
            HomeGoalkeeperId = request.HomeGoalkeeperId,
            AwayGoalkeeperId = request.AwayGoalkeeperId,
            HomeCaptainId = request.HomeCaptainId,
            AwayCaptainId = request.AwayCaptainId,
            Goals = request.Goals?.Select(g => g.FromRequest(0)).ToList(),
            Cards = (request.YellowCards == null && request.RedCards == null) 
                ? null 
                : (request.YellowCards?.Select(c => c.FromRequest(0, FootballCardType.Yellow)) ?? Enumerable.Empty<FootballCard>())
                    .Concat(request.RedCards?.Select(c => c.FromRequest(0, FootballCardType.Red)) ?? Enumerable.Empty<FootballCard>())
                    .ToList()
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
            HomePenaltyScore = entity.HomePenaltyScore,
            AwayPenaltyScore = entity.AwayPenaltyScore,
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
            Goals = entity.Goals?.Select(g => g.ToResponse()).ToList(),
            YellowCards = entity.Cards?.Where(c => c.Type == FootballCardType.Yellow).Select(c => c.ToResponse()).ToList(),
            RedCards = entity.Cards?.Where(c => c.Type == FootballCardType.Red).Select(c => c.ToResponse()).ToList()
        };
    }
}
