using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Chess;

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
        Players = chessEvent.Players?.Select(p => p.ToDto()).ToList() ?? []
        };
    }

}
