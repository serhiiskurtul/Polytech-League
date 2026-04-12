using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Armwrestling;
using PolytechLeague.Domain.Entities.Armwrestling.Enums;
using PolytechLeague.Application.Dto.ChessDto; // For EventPhotoDto
using PolytechLeague.Application.Dto.GeneralDto;

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

    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public string? TimeControl { get; set; }
    public int ParticipantsCount { get; set; }

    public long? FirstPlaceId { get; set; }
    public long? SecondPlaceId { get; set; }
    public long? ThirdPlaceId { get; set; }
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

    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public string? TimeControl { get; set; }
    public int ParticipantsCount { get; set; }

    public PlayerDto? FirstPlace { get; set; }
    public PlayerDto? SecondPlace { get; set; }
    public PlayerDto? ThirdPlace { get; set; }

    public ICollection<PlayerDto> Players { get; set; } = new List<PlayerDto>();
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
            Category = request.Category,
            Rules = request.Rules,
            Prizes = request.Prizes,
            HeadRefereeName = request.HeadRefereeName,
            Referees = request.Referees,
            Schedule = request.Schedule,
            TimeControl = request.TimeControl,
            ParticipantsCount = request.ParticipantsCount,
            FirstPlaceId = request.FirstPlaceId,
            SecondPlaceId = request.SecondPlaceId,
            ThirdPlaceId = request.ThirdPlaceId
        };
    }

    public static ArmwrestlingEventResponse ToResponse(this ArmwrestlingEvent entity)
    {
        var response = new ArmwrestlingEventResponse
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
            Matches = entity.Matches?.Select(m => m.ToResponse()).ToList(),
            Rules = entity.Rules,
            Prizes = entity.Prizes,
            HeadRefereeName = entity.HeadRefereeName,
            Referees = entity.Referees,
            Schedule = entity.Schedule,
            TimeControl = entity.TimeControl,
            ParticipantsCount = entity.ParticipantsCount,
            FirstPlace = entity.FirstPlace?.ToDto(),
            SecondPlace = entity.SecondPlace?.ToDto(),
            ThirdPlace = entity.ThirdPlace?.ToDto()
        };

        // Extract unique players from matches to populate the "Players" virtual collection
        if (response.Matches != null)
        {
            var playersMap = new Dictionary<long, PlayerDto?>();
            foreach (var match in response.Matches)
            {
                if (match.Player1 != null && !playersMap.ContainsKey(match.Player1Id))
                    playersMap[match.Player1Id] = match.Player1;
                if (match.Player2 != null && !playersMap.ContainsKey(match.Player2Id))
                    playersMap[match.Player2Id] = match.Player2;
            }
            response.Players = playersMap.Values.Where(p => p != null).Select(p => p!).ToList();
        }

        return response;
    }
}
