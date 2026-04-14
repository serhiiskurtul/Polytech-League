using System.Text.Json.Serialization;
using PolytechLeague.Application.Dto;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.BeachVolleyball;
using PolytechLeague.Domain.Entities.BeachVolleyball.Enums;

namespace PolytechLeague.Application.Dto.BeachVolleyballDto;

public class BeachVolleyballEventRequest
{
    public required string Name { get; set; }
    public required DateTime Date { get; set; }
    public required string Location { get; set; }
    public required ICollection<long> RefereeIds { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CompetitionLevel Level { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required BeachVolleyballTournamentType TournamentType { get; set; }

    public required string Description { get; set; }
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }

    public long? FirstPlaceTeamId { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
}

public class BeachVolleyballEventResponse
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required DateTime Date { get; set; }
    public required string Location { get; set; }
    public ICollection<EventPhotoDto>? Photos { get; set; }
    public required ICollection<long> RefereeIds { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CompetitionLevel Level { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required BeachVolleyballTournamentType TournamentType { get; set; }

    public required string Description { get; set; }
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }

    public BeachVolleyballTeamResponse? FirstPlaceTeam { get; set; }
    public BeachVolleyballTeamResponse? SecondPlaceTeam { get; set; }
    public BeachVolleyballTeamResponse? ThirdPlaceTeam { get; set; }

    public ICollection<BeachVolleyballTeamResponse>? Teams { get; set; }
    public ICollection<BeachVolleyballMatchResponse>? Matches { get; set; }
}

public static class BeachVolleyballEventDtoExtensions
{
    public static BeachVolleyballEvent FromRequest(this BeachVolleyballEventRequest request)
    {
        return new BeachVolleyballEvent
        {
            Name = request.Name,
            Date = request.Date,
            Location = request.Location,
            RefereeIds = request.RefereeIds,
            Level = request.Level,
            TournamentType = request.TournamentType,
            Description = request.Description,
            Rules = request.Rules,
            Prizes = request.Prizes,
            HeadRefereeName = request.HeadRefereeName,
            Referees = request.Referees,
            Schedule = request.Schedule,
            FirstPlaceTeamId = request.FirstPlaceTeamId,
            SecondPlaceTeamId = request.SecondPlaceTeamId,
            ThirdPlaceTeamId = request.ThirdPlaceTeamId
        };
    }

    public static BeachVolleyballEventResponse ToResponse(this BeachVolleyballEvent entity)
    {
        return new BeachVolleyballEventResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Date = entity.Date,
            Location = entity.Location,
            Photos = entity.Photos?.Select(p => p.ToDto()).ToList(),
            RefereeIds = entity.RefereeIds,
            Level = entity.Level,
            TournamentType = entity.TournamentType,
            Description = entity.Description,
            Rules = entity.Rules,
            Prizes = entity.Prizes,
            HeadRefereeName = entity.HeadRefereeName,
            Referees = entity.Referees,
            Schedule = entity.Schedule,
            FirstPlaceTeam = entity.FirstPlaceTeam?.ToResponse(),
            SecondPlaceTeam = entity.SecondPlaceTeam?.ToResponse(),
            ThirdPlaceTeam = entity.ThirdPlaceTeam?.ToResponse(),
            Teams = entity.Teams?.Select(t => t.ToResponse()).ToList(),
            Matches = entity.Matches?.Select(m => m.ToResponse()).ToList()
        };
    }
}
