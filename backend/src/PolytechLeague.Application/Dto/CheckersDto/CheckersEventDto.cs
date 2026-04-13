using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Application.Dto.ChessDto;
using PolytechLeague.Application.Dto.GeneralDto;

namespace PolytechLeague.Application.Dto.CheckersDto;

public class CheckersEventRequest
{
    public required string Name { get; set; }
    public required DateTime Date { get; set; }
    public required string Location { get; set; }
    public required ICollection<long> RefereeIds { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CompetitionLevel Level { get; set; }

    public required string Description { get; set; }
    public required string TimeControl { get; set; }
    public required int ParticipantsCount { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CheckersSystem System { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CheckersVariant Variant { get; set; }

    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsTeam { get; set; }

    public long? FirstPlaceId { get; set; }
    public long? SecondPlaceId { get; set; }
    public long? ThirdPlaceId { get; set; }

    public long? FirstPlaceTeamId { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
}

public class CheckersEventResponse
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
    public required string TimeControl { get; set; }
    public required int ParticipantsCount { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CheckersSystem System { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CheckersVariant Variant { get; set; }

    public required ICollection<CheckersGameDto> Games { get; set; }
    public required ICollection<CheckersEventParticipantDto> Players { get; set; }
    public ICollection<CheckersTeamResponse>? Teams { get; set; }

    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsTeam { get; set; }

    public PlayerDto? FirstPlace { get; set; }
    public PlayerDto? SecondPlace { get; set; }
    public PlayerDto? ThirdPlace { get; set; }

    public CheckersTeamResponse? FirstPlaceTeam { get; set; }
    public CheckersTeamResponse? SecondPlaceTeam { get; set; }
    public CheckersTeamResponse? ThirdPlaceTeam { get; set; }
}

public static class CheckersEventDtoExtension
{
    public static CheckersEvent FromRequest(this CheckersEventRequest checkersEventRequest)
    {
        return new CheckersEvent
        {
            Name = checkersEventRequest.Name,
            Date = checkersEventRequest.Date,
            Location = checkersEventRequest.Location,
            RefereeIds = checkersEventRequest.RefereeIds,
            Level = checkersEventRequest.Level,
            Description = checkersEventRequest.Description,
            TimeControl = checkersEventRequest.TimeControl,
            ParticipantsCount = checkersEventRequest.ParticipantsCount,
            System = checkersEventRequest.System,
            Variant = checkersEventRequest.Variant,
            Rules = checkersEventRequest.Rules,
            Prizes = checkersEventRequest.Prizes,
            HeadRefereeName = checkersEventRequest.HeadRefereeName,
            Referees = checkersEventRequest.Referees,
            Schedule = checkersEventRequest.Schedule,
            IsTeam = checkersEventRequest.IsTeam,
            FirstPlaceId = checkersEventRequest.FirstPlaceId,
            SecondPlaceId = checkersEventRequest.SecondPlaceId,
            ThirdPlaceId = checkersEventRequest.ThirdPlaceId,
            FirstPlaceTeamId = checkersEventRequest.FirstPlaceTeamId,
            SecondPlaceTeamId = checkersEventRequest.SecondPlaceTeamId,
            ThirdPlaceTeamId = checkersEventRequest.ThirdPlaceTeamId
        };
    }

    public static CheckersEventResponse ToResponse(this CheckersEvent checkersEvent)
    {
        return new CheckersEventResponse
        {
            Id = checkersEvent.Id,
            Name = checkersEvent.Name,
            Date = checkersEvent.Date,
            Location = checkersEvent.Location,
            Photos = checkersEvent.Photos?.Select(p => p.ToDto()).ToList(),
            RefereeIds = checkersEvent.RefereeIds,
            Level = checkersEvent.Level,
            Description = checkersEvent.Description,
            TimeControl = checkersEvent.TimeControl,
            ParticipantsCount = checkersEvent.ParticipantsCount,
            System = checkersEvent.System,
            Variant = checkersEvent.Variant,
            Games = checkersEvent.Games?.Select(g => g.ToDto()).ToList() ?? [],
            Players = checkersEvent.Players?.Select(p => p.ToDto()).ToList() ?? [],
            Teams = checkersEvent.Teams?.Select(t => t.ToResponse()).ToList(),
            Rules = checkersEvent.Rules,
            Prizes = checkersEvent.Prizes,
            HeadRefereeName = checkersEvent.HeadRefereeName,
            Referees = checkersEvent.Referees,
            Schedule = checkersEvent.Schedule,
            IsTeam = checkersEvent.IsTeam,
            FirstPlace = checkersEvent.FirstPlace?.ToDto(),
            SecondPlace = checkersEvent.SecondPlace?.ToDto(),
            ThirdPlace = checkersEvent.ThirdPlace?.ToDto(),
            FirstPlaceTeam = checkersEvent.FirstPlaceTeam?.ToResponse(),
            SecondPlaceTeam = checkersEvent.SecondPlaceTeam?.ToResponse(),
            ThirdPlaceTeam = checkersEvent.ThirdPlaceTeam?.ToResponse()
        };
    }
}
