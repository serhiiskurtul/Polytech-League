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
    
    public string? Description { get; set; }

    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ArmwrestlingHand Hand { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ArmwrestlingWeightClass WeightClass { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required EventCategory Category { get; set; }

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
    public ArmwrestlingTournamentType? TournamentType { get; set; } = ArmwrestlingTournamentType.SingleElimination;

    public bool IsModerated { get; set; }
    public bool IsPublic { get; set; }
    public long? MvpId { get; set; }
    public long? SeasonId { get; set; }
}


public class ArmwrestlingEventResponse
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required DateTime Date { get; set; }
    public required string Location { get; set; }
    public ICollection<EventPhotoDto>? Photos { get; set; }
    public string? MainPhotoUrl { get; set; }
    public required ICollection<long> RefereeIds { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CompetitionLevel Level { get; set; }
    public string? Description { get; set; }

    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ArmwrestlingHand Hand { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ArmwrestlingWeightClass WeightClass { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required EventCategory Category { get; set; }
    
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
    public ArmwrestlingTournamentType? TournamentType { get; set; }

    public bool IsModerated { get; set; }
    public bool IsPublic { get; set; }

    public ICollection<ArmwrestlingEventParticipantDto> Participants { get; set; } = new List<ArmwrestlingEventParticipantDto>();
    public long? MvpId { get; set; }
    public string? MvpName { get; set; }
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
            MvpId = request.MvpId,
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
            MainPhotoUrl = entity.MainPhotoUrl,
            RefereeIds = entity.RefereeIds,
            Level = entity.Level,
            Description = entity.Description,
            RegistrationStart = entity.RegistrationStart,
            RegistrationEnd = entity.RegistrationEnd,
            EndDate = entity.EndDate,
            Hand = entity.Hand,
            WeightClass = entity.WeightClass,
            Category = entity.Category,
            Matches = entity.Matches?.Select(m => m.ToResponse(entity.Players)).ToList(),
            Rules = entity.Rules,
            Prizes = entity.Prizes,
            HeadRefereeName = entity.HeadRefereeName,
            Referees = entity.Referees,
            Schedule = entity.Schedule,
            TimeControl = entity.TimeControl,
            ParticipantsCount = entity.ParticipantsCount,
            TournamentType = entity.TournamentType,
            IsModerated = entity.IsModerated,
            IsPublic = entity.IsPublic,
            MvpId = entity.MvpId,
            MvpName = entity.Mvp != null ? $"{entity.Mvp.FirstName} {entity.Mvp.LastName}" : null,
            SeasonId = entity.SeasonId

        };

        if (entity.Players != null && entity.Players.Any())
        {
            response.Participants = entity.Players
                .Select(p => p.ToDto())
                .ToList();

            if (entity.FirstPlaceId != null)
            {
                var p = entity.Players.FirstOrDefault(x => x.PlayerId == entity.FirstPlaceId);
                if (p != null) response.FirstPlace = p.ToWinnerDto();
                else if (entity.FirstPlace != null) response.FirstPlace = entity.FirstPlace.ToDto();
            }
            if (entity.SecondPlaceId != null)
            {
                var p = entity.Players.FirstOrDefault(x => x.PlayerId == entity.SecondPlaceId);
                if (p != null) response.SecondPlace = p.ToWinnerDto();
                else if (entity.SecondPlace != null) response.SecondPlace = entity.SecondPlace.ToDto();
            }
            if (entity.ThirdPlaceId != null)
            {
                var p = entity.Players.FirstOrDefault(x => x.PlayerId == entity.ThirdPlaceId);
                if (p != null) response.ThirdPlace = p.ToWinnerDto();
                else if (entity.ThirdPlace != null) response.ThirdPlace = entity.ThirdPlace.ToDto();
            }
        }
        else
        {
            response.FirstPlace = entity.FirstPlace?.ToDto();
            response.SecondPlace = entity.SecondPlace?.ToDto();
            response.ThirdPlace = entity.ThirdPlace?.ToDto();

            if (entity.Matches != null)
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
        }

        return response;
    }

    private static PlayerDto ToWinnerDto(this ArmwrestlingEventParticipant participant)
    {
        return new PlayerDto
        {
            Id = participant.PlayerId,
            FirstName = participant.PlayerFirstName ?? participant.Player?.FirstName ?? "Unknown",
            LastName = participant.PlayerLastName ?? participant.Player?.LastName ?? "",
            Group = participant.GroupName ?? participant.Player?.Group ?? "Unknown",
            PhotoUrl = participant.Player?.PhotoUrl
        };
    }
}
