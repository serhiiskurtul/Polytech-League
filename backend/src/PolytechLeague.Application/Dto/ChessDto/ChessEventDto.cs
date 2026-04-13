using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Chess;
using PolytechLeague.Application.Dto.GeneralDto;

namespace PolytechLeague.Application.Dto.ChessDto;

public class ChessEventRequest
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
    public required ChessSystem System { get; set; }
    
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

public class ChessEventResponse
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
    public required ChessSystem System { get; set; }
    public required ICollection<ChessGameDto> Games { get; set; }
    public required ICollection<ChessEventParticipantDto> Players { get; set; }
    public ICollection<ChessTeamResponse>? Teams { get; set; }
    
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsTeam { get; set; }

    public PlayerDto? FirstPlace { get; set; }
    public PlayerDto? SecondPlace { get; set; }
    public PlayerDto? ThirdPlace { get; set; }

    public ChessTeamResponse? FirstPlaceTeam { get; set; }
    public ChessTeamResponse? SecondPlaceTeam { get; set; }
    public ChessTeamResponse? ThirdPlaceTeam { get; set; }
}

public static class ChessEventDtoExtension
{
    public static ChessEvent FromRequest(this ChessEventRequest chessEventRequest)
    {
        return new ChessEvent
        {
            Name = chessEventRequest.Name,
            Date = chessEventRequest.Date,
            Location = chessEventRequest.Location,
            RefereeIds = chessEventRequest.RefereeIds,
            Level = chessEventRequest.Level,
            Description = chessEventRequest.Description,
            TimeControl = chessEventRequest.TimeControl,
            ParticipantsCount = chessEventRequest.ParticipantsCount,
            System = chessEventRequest.System,
            Rules = chessEventRequest.Rules,
            Prizes = chessEventRequest.Prizes,
            HeadRefereeName = chessEventRequest.HeadRefereeName,
            Referees = chessEventRequest.Referees,
            Schedule = chessEventRequest.Schedule,
            IsTeam = chessEventRequest.IsTeam,
            FirstPlaceId = chessEventRequest.FirstPlaceId,
            SecondPlaceId = chessEventRequest.SecondPlaceId,
            ThirdPlaceId = chessEventRequest.ThirdPlaceId,
            FirstPlaceTeamId = chessEventRequest.FirstPlaceTeamId,
            SecondPlaceTeamId = chessEventRequest.SecondPlaceTeamId,
            ThirdPlaceTeamId = chessEventRequest.ThirdPlaceTeamId
        };
    }

    public static ChessEventResponse ToResponse(this ChessEvent chessEvent)
    {
        return new ChessEventResponse
        {
            Id = chessEvent.Id,
            Name = chessEvent.Name,
            Date = chessEvent.Date,
            Location = chessEvent.Location,
            Photos = chessEvent.Photos?.Select(p => p.ToDto()).ToList(),
            RefereeIds = chessEvent.RefereeIds,
            Level = chessEvent.Level,
            Description = chessEvent.Description,
            TimeControl = chessEvent.TimeControl,
            ParticipantsCount = chessEvent.ParticipantsCount,
            System = chessEvent.System,
            Games = chessEvent.Games?.Select(g => g.ToDto()).ToList() ?? [],
            Players = chessEvent.Players?.Select(p => p.ToDto()).ToList() ?? [],
            Teams = chessEvent.Teams?.Select(t => t.ToResponse()).ToList(),
            Rules = chessEvent.Rules,
            Prizes = chessEvent.Prizes,
            HeadRefereeName = chessEvent.HeadRefereeName,
            Referees = chessEvent.Referees,
            Schedule = chessEvent.Schedule,
            IsTeam = chessEvent.IsTeam,
            FirstPlace = chessEvent.FirstPlace?.ToDto(),
            SecondPlace = chessEvent.SecondPlace?.ToDto(),
            ThirdPlace = chessEvent.ThirdPlace?.ToDto(),
            FirstPlaceTeam = chessEvent.FirstPlaceTeam?.ToResponse(),
            SecondPlaceTeam = chessEvent.SecondPlaceTeam?.ToResponse(),
            ThirdPlaceTeam = chessEvent.ThirdPlaceTeam?.ToResponse()
        };
    }
}
