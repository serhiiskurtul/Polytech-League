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
            Description = request.Description
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
            Matches = entity.Matches?.Select(m => m.ToResponse()).ToList()
        };
    }
}
