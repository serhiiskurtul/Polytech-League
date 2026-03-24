using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Badminton;
using PolytechLeague.Domain.Entities.Badminton.Enums;

namespace PolytechLeague.Application.Dto.BadmintonDto;

public class BadmintonEventRequest
{
    public required string Name { get; set; }
    public required DateTime Date { get; set; }
    public required string Location { get; set; }
    public required ICollection<long> RefereeIds { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CompetitionLevel Level { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required BadmintonTournamentType TournamentType { get; set; }
    
    public required string Description { get; set; }
}

public class BadmintonEventResponse
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
    public required BadmintonTournamentType TournamentType { get; set; }

    public required string Description { get; set; }
    
    public ICollection<BadmintonMatchResponse>? Matches { get; set; }
}

public class BadmintonMatchResponse
{
    public long Id { get; set; }
    public long Player1Id { get; set; }
    public BadmintonPlayerResponse? Player1 { get; set; }
    public long Player2Id { get; set; }
    public BadmintonPlayerResponse? Player2 { get; set; }
    public long? WinnerId { get; set; }
    public BadmintonPlayerResponse? Winner { get; set; }
    public string? Stage { get; set; }
    public int CourtNumber { get; set; }
    public int SetsToWin { get; set; }
    public ICollection<BadmintonSetResponse>? Sets { get; set; }
}

public class BadmintonSetResponse
{
    public long Id { get; set; }
    public int SetNumber { get; set; }
    public int Player1Score { get; set; }
    public int Player2Score { get; set; }
}

public class BadmintonPlayerResponse
{
    public long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}

public static class BadmintonDtoExtensions
{
    public static BadmintonEvent FromRequest(this BadmintonEventRequest request)
    {
        return new BadmintonEvent
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

    public static BadmintonEventResponse ToResponse(this BadmintonEvent entity)
    {
        return new BadmintonEventResponse
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

    public static BadmintonMatchResponse ToResponse(this BadmintonMatch entity)
    {
        return new BadmintonMatchResponse
        {
            Id = entity.Id,
            Player1Id = entity.Player1Id,
            Player1 = entity.Player1?.ToResponse(),
            Player2Id = entity.Player2Id,
            Player2 = entity.Player2?.ToResponse(),
            WinnerId = entity.WinnerId,
            Winner = entity.Winner?.ToResponse(),
            Stage = entity.Stage,
            CourtNumber = entity.CourtNumber,
            SetsToWin = entity.SetsToWin,
            Sets = entity.Sets?.Select(s => s.ToResponse()).ToList()
        };
    }

    public static BadmintonSetResponse ToResponse(this BadmintonSet entity)
    {
        return new BadmintonSetResponse
        {
            Id = entity.Id,
            SetNumber = entity.SetNumber,
            Player1Score = entity.Player1Score,
            Player2Score = entity.Player2Score
        };
    }

    public static BadmintonPlayerResponse ToResponse(this BadmintonPlayer entity)
    {
        return new BadmintonPlayerResponse
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName
        };
    }
}
