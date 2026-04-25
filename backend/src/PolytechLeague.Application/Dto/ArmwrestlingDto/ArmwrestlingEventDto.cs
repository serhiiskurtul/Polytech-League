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
    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }

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

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ArmwrestlingTournamentType TournamentType { get; set; } = ArmwrestlingTournamentType.SingleElimination;
    public bool IsModerated { get; set; }
    public bool IsPublic { get; set; }
    public long? SeasonId { get; set; }
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
    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }

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

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ArmwrestlingTournamentType TournamentType { get; set; }
    public bool IsModerated { get; set; }
    public bool IsPublic { get; set; }

    public ICollection<ArmwrestlingEventParticipantDto> Participants { get; set; } = new List<ArmwrestlingEventParticipantDto>();
    public long? SeasonId { get; set; }
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
            RegistrationStart = request.RegistrationStart,
            RegistrationEnd = request.RegistrationEnd,
            EndDate = request.EndDate,
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
            ThirdPlaceId = request.ThirdPlaceId,
            TournamentType = request.TournamentType,
            IsModerated = request.IsModerated,
            IsPublic = request.IsPublic,
            SeasonId = request.SeasonId
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
            RegistrationStart = entity.RegistrationStart,
            RegistrationEnd = entity.RegistrationEnd,
            EndDate = entity.EndDate,
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
            ThirdPlace = entity.ThirdPlace?.ToDto(),
            TournamentType = entity.TournamentType,
            IsModerated = entity.IsModerated,
            IsPublic = entity.IsPublic,
            SeasonId = entity.SeasonId
        };

        // Populate Participants from the actual participants collection
        if (entity.Players != null && entity.Players.Any())
        {
            response.Participants = entity.Players
                .Select(p => p.ToDto())
                .ToList();
        }
        else if (entity.Matches != null)
        {
            // Fallback for older data or if participants weren't properly migrated
            var playersMap = new Dictionary<long, ArmwrestlingEventParticipantDto>();
            foreach (var match in entity.Matches)
            {
                if (match.Player1 != null && !playersMap.ContainsKey(match.Player1Id))
                    playersMap[match.Player1Id] = new ArmwrestlingEventParticipantDto { PlayerId = match.Player1Id, PlayerName = $"{match.Player1.FirstName} {match.Player1.LastName}" };
                if (match.Player2 != null && !playersMap.ContainsKey(match.Player2Id))
                    playersMap[match.Player2Id] = new ArmwrestlingEventParticipantDto { PlayerId = match.Player2Id, PlayerName = $"{match.Player2.FirstName} {match.Player2.LastName}" };
            }
            response.Participants = playersMap.Values.ToList();
        }

        return response;
    }
}
