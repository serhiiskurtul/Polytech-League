using System.Text.Json.Serialization;
using PolytechLeague.Application.Dto;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Volleyball;
using PolytechLeague.Domain.Entities.Volleyball.Enums;

namespace PolytechLeague.Application.Dto.VolleyballDto;

public class VolleyballEventRequest
{
    public required string Name { get; set; }
    public required DateTime Date { get; set; }
    public required string Location { get; set; }
    public required ICollection<long> RefereeIds { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CompetitionLevel Level { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required VolleyballTournamentType TournamentType { get; set; }

    public required string Description { get; set; }
}

public class VolleyballEventResponse
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
    public required VolleyballTournamentType TournamentType { get; set; }

    public required string Description { get; set; }

    public ICollection<VolleyballMatchResponse>? Matches { get; set; }
}

public static class VolleyballEventDtoExtensions
{
    public static VolleyballEvent FromRequest(this VolleyballEventRequest request)
    {
        return new VolleyballEvent
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

    public static VolleyballEventResponse ToResponse(this VolleyballEvent entity)
    {
        return new VolleyballEventResponse
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
