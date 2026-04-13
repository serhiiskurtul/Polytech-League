using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.BeachVolleyball;

namespace PolytechLeague.Application.Dto.BeachVolleyballDto;

public class BeachVolleyballSetDto
{
    public long Id { get; set; }
    public int SetNumber { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
}

public class BeachVolleyballMatchRequest
{
    public long EventId { get; set; }
    public long HomeTeamId { get; set; }
    public long AwayTeamId { get; set; }
    public int SetsToWin { get; set; }
    public int? HomeSetsWon { get; set; }
    public int? AwaySetsWon { get; set; }
    public long? WinnerTeamId { get; set; }
    public string? Stage { get; set; }
    public ICollection<long>? HomeRosterIds { get; set; }
    public ICollection<long>? AwayRosterIds { get; set; }
}

public class BeachVolleyballMatchResponse
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public BeachVolleyballEventResponse? Event { get; set; }

    public long HomeTeamId { get; set; }
    public BeachVolleyballTeamResponse? HomeTeam { get; set; }

    public long AwayTeamId { get; set; }
    public BeachVolleyballTeamResponse? AwayTeam { get; set; }

    public int SetsToWin { get; set; }
    public int? HomeSetsWon { get; set; }
    public int? AwaySetsWon { get; set; }

    public long? WinnerTeamId { get; set; }
    public BeachVolleyballTeamResponse? WinnerTeam { get; set; }

    public string? Stage { get; set; }

    public ICollection<PlayerDto>? HomeRoster { get; set; }
    public ICollection<PlayerDto>? AwayRoster { get; set; }
    public ICollection<BeachVolleyballSetDto>? Sets { get; set; }
}

public static class BeachVolleyballMatchDtoExtensions
{
    public static BeachVolleyballMatch FromRequest(this BeachVolleyballMatchRequest request)
    {
        return new BeachVolleyballMatch
        {
            EventId = request.EventId,
            HomeTeamId = request.HomeTeamId,
            AwayTeamId = request.AwayTeamId,
            SetsToWin = request.SetsToWin,
            HomeSetsWon = request.HomeSetsWon,
            AwaySetsWon = request.AwaySetsWon,
            WinnerTeamId = request.WinnerTeamId,
            Stage = request.Stage,
            HomeRosterIds = request.HomeRosterIds,
            AwayRosterIds = request.AwayRosterIds
        };
    }

    public static BeachVolleyballMatchResponse ToResponse(this BeachVolleyballMatch entity)
    {
        return new BeachVolleyballMatchResponse
        {
            Id = entity.Id,
            EventId = entity.EventId,
            Event = entity.Event?.ToResponse(),
            HomeTeamId = entity.HomeTeamId,
            HomeTeam = entity.HomeTeam?.ToResponse(),
            AwayTeamId = entity.AwayTeamId,
            AwayTeam = entity.AwayTeam?.ToResponse(),
            SetsToWin = entity.SetsToWin,
            HomeSetsWon = entity.HomeSetsWon,
            AwaySetsWon = entity.AwaySetsWon,
            WinnerTeamId = entity.WinnerTeamId,
            WinnerTeam = entity.WinnerTeam?.ToResponse(),
            Stage = entity.Stage,
            HomeRoster = entity.HomeRoster?.Select(p => p.ToDto()).ToList(),
            AwayRoster = entity.AwayRoster?.Select(p => p.ToDto()).ToList(),
            Sets = entity.Sets?.Select(s => s.ToDto()).ToList()
        };
    }

    public static BeachVolleyballSetDto ToDto(this BeachVolleyballSet entity)
    {
        return new BeachVolleyballSetDto
        {
            Id = entity.Id,
            SetNumber = entity.SetNumber,
            HomeScore = entity.HomeScore,
            AwayScore = entity.AwayScore
        };
    }
}
