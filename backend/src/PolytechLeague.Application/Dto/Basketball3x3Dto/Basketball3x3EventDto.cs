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
    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsModerated { get; set; }
    public bool IsPublic { get; set; }
    public bool IsTeam { get; set; }

    public long? FirstPlaceTeamId { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public long? SeasonId { get; set; }
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
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }

    public Basketball3x3TeamResponse? FirstPlaceTeam { get; set; }
    public Basketball3x3TeamResponse? SecondPlaceTeam { get; set; }
    public Basketball3x3TeamResponse? ThirdPlaceTeam { get; set; }
    public ICollection<Basketball3x3TeamResponse>? Teams { get; set; }
    public ICollection<Basketball3x3MatchResponse>? Matches { get; set; }
    public long? SeasonId { get; set; }
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
            Description = request.Description,
            RegistrationStart = request.RegistrationStart,
            RegistrationEnd = request.RegistrationEnd,
            EndDate = request.EndDate,
            Rules = request.Rules,
            Prizes = request.Prizes,
            HeadRefereeName = request.HeadRefereeName,
            Referees = request.Referees,
            Schedule = request.Schedule,
            IsModerated = request.IsModerated,
            IsPublic = request.IsPublic,
            IsTeam = request.IsTeam,
            FirstPlaceTeamId = request.FirstPlaceTeamId,
            SecondPlaceTeamId = request.SecondPlaceTeamId,
            ThirdPlaceTeamId = request.ThirdPlaceTeamId,
            SeasonId = request.SeasonId
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
            Rules = entity.Rules,
            Prizes = entity.Prizes,
            HeadRefereeName = entity.HeadRefereeName,
            Referees = entity.Referees,
            Schedule = entity.Schedule,
            FirstPlaceTeam = entity.FirstPlaceTeam?.ToResponse(),
            SecondPlaceTeam = entity.SecondPlaceTeam?.ToResponse(),
            ThirdPlaceTeam = entity.ThirdPlaceTeam?.ToResponse(),
            Teams = entity.Teams?.Select(t => t.ToResponse()).ToList(),
            Matches = entity.Matches?.Select(m => m.ToResponse()).ToList(),
            SeasonId = entity.SeasonId
        };
    }
}
