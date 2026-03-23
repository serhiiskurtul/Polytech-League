using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Armwrestling;
using PolytechLeague.Domain.Entities.Armwrestling.Enums;
using PolytechLeague.Application.Dto.ChessDto; // For EventPhotoDto

namespace PolytechLeague.Application.Dto.ArmwrestlingDto;

public class ArmwrestlingEventRequest
{
    public required string Name { get; set; }
    public required DateTime Date { get; set; }
    public required string Location { get; set; }
    public required ICollection<long> RefereeIds { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CompetitionLevel Level { get; set; }
    
    public required string Description { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ArmwrestlingHand Hand { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ArmwrestlingWeightClass WeightClass { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ArmwrestlingCategory Category { get; set; }
}

public class ArmwrestlingEventResponse
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

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ArmwrestlingHand Hand { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ArmwrestlingWeightClass WeightClass { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ArmwrestlingCategory Category { get; set; }
    
    public ICollection<ArmwrestlingMatchResponse>? Matches { get; set; }
}

public static class ArmwrestlingEventDtoExtensions
{
    public static ArmwrestlingEvent FromRequest(this ArmwrestlingEventRequest request)
    {
        return new ArmwrestlingEvent
        {
            Name = request.Name,
            Date = request.Date,
            Location = request.Location,
            RefereeIds = request.RefereeIds,
            Level = request.Level,
            Description = request.Description,
            Hand = request.Hand,
            WeightClass = request.WeightClass,
            Category = request.Category
        };
    }

    public static ArmwrestlingEventResponse ToResponse(this ArmwrestlingEvent entity)
    {
        return new ArmwrestlingEventResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Date = entity.Date,
            Location = entity.Location,
            Photos = entity.Photos?.Select(p => p.ToDto()).ToList(),
            RefereeIds = entity.RefereeIds,
            Level = entity.Level,
            Description = entity.Description,
            Hand = entity.Hand,
            WeightClass = entity.WeightClass,
            Category = entity.Category,
            Matches = entity.Matches?.Select(m => m.ToResponse()).ToList()
        };
    }
}
