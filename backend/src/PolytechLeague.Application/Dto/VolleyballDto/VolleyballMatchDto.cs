using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Volleyball;

namespace PolytechLeague.Application.Dto.VolleyballDto;

public class VolleyballSetDto
{
    public long Id { get; set; }
    public int SetNumber { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
}

public class VolleyballMatchRequest
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

public class VolleyballMatchResponse
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public VolleyballEventResponse? Event { get; set; }

    public long HomeTeamId { get; set; }
    public VolleyballTeamResponse? HomeTeam { get; set; }

    public long AwayTeamId { get; set; }
    public VolleyballTeamResponse? AwayTeam { get; set; }

    public int SetsToWin { get; set; }
    public int? HomeSetsWon { get; set; }
    public int? AwaySetsWon { get; set; }

    public long? WinnerTeamId { get; set; }
    public VolleyballTeamResponse? WinnerTeam { get; set; }

    public string? Stage { get; set; }

    public ICollection<PlayerDto>? HomeRoster { get; set; }
    public ICollection<PlayerDto>? AwayRoster { get; set; }
    public ICollection<VolleyballSetDto>? Sets { get; set; }
}

public static class VolleyballMatchDtoExtensions
{
    public static VolleyballMatch FromRequest(this VolleyballMatchRequest request)
    {
        return new VolleyballMatch
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

    public static VolleyballMatchResponse ToResponse(this VolleyballMatch entity)
    {
        return new VolleyballMatchResponse
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

    public static VolleyballSetDto ToDto(this VolleyballSet entity)
    {
        return new VolleyballSetDto
        {
            Id = entity.Id,
            SetNumber = entity.SetNumber,
            HomeScore = entity.HomeScore,
            AwayScore = entity.AwayScore
        };
    }
}
