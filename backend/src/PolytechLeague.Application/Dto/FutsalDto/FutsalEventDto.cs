using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Futsal;

namespace PolytechLeague.Application.Dto.FutsalDto;

public class FutsalEventRequest
{
    public required string Name { get; set; }
    public required DateTime Date { get; set; }
    public required string Location { get; set; }
    public required ICollection<long> RefereeIds { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CompetitionLevel Level { get; set; }
    
    public required string Description { get; set; }
}

public class FutsalEventResponse
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required DateTime Date { get; set; }
    public required string Location { get; set; }
    public ICollection<EventPhotoDto>? Photos { get; set; }
    public required ICollection<long> RefereeIds { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CompetitionLevel Level { get; set; }
    public required string Description { get; set; }
    
    public ICollection<FutsalMatchResponse>? Matches { get; set; }
}

public static class FutsalEventDtoExtensions
{
    public static FutsalEvent FromRequest(this FutsalEventRequest request)
    {
        return new FutsalEvent
        {
            Name = request.Name,
            Date = request.Date,
            Location = request.Location,
            RefereeIds = request.RefereeIds,
            Level = request.Level,
            Description = request.Description
        };
    }

    public static FutsalEventResponse ToResponse(this FutsalEvent entity)
    {
        return new FutsalEventResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Date = entity.Date,
            Location = entity.Location,
            Photos = entity.Photos?.Select(p => p.ToDto()).ToList(),
            RefereeIds = entity.RefereeIds,
            Level = entity.Level,
            Description = entity.Description,
            Matches = entity.Matches?.Select(m => m.ToResponse()).ToList()
        };
    }
}
