using PolytechLeague.Domain.Entities.Futsal;

namespace PolytechLeague.Application.Dto.FutsalDto;

public class FutsalMatchRequest
{
    public long EventId { get; set; }
    public long HomeTeamId { get; set; }
    public long AwayTeamId { get; set; }
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
    public long? WinnerTeamId { get; set; }
    public string? Stage { get; set; }
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
            Stage = request.Stage
        };
    }

    public static FutsalMatchResponse ToResponse(this FutsalMatch entity)
    {
        return new FutsalMatchResponse
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
            Stage = entity.Stage
        };
    }
}
