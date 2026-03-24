using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Basketball3x3;
using PolytechLeague.Domain.Entities.Basketball3x3.Enums;

namespace PolytechLeague.Application.Dto.Basketball3x3Dto;

public class Basketball3x3EventRequest
{
    public required string Name { get; set; }
    public required DateTime Date { get; set; }
    public required string Location { get; set; }
    public required ICollection<long> RefereeIds { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CompetitionLevel Level { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Basketball3x3TournamentType TournamentType { get; set; }
    
    public required string Description { get; set; }
}

public class Basketball3x3EventResponse
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
    public required Basketball3x3TournamentType TournamentType { get; set; }

    public required string Description { get; set; }
    
    public ICollection<Basketball3x3MatchResponse>? Matches { get; set; }
}

public static class Basketball3x3EventDtoExtensions
{
    public static Basketball3x3Event FromRequest(this Basketball3x3EventRequest request)
    {
        return new Basketball3x3Event
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

    public static Basketball3x3EventResponse ToResponse(this Basketball3x3Event entity)
    {
        return new Basketball3x3EventResponse
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
