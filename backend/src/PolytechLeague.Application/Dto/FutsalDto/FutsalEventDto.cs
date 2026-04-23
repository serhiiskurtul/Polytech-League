using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Futsal;
using PolytechLeague.Domain.Entities.Futsal.Enums;
using PolytechLeague.Application.Dto.ChessDto;

namespace PolytechLeague.Application.Dto.FutsalDto;

public class FutsalEventRequest
{
    public required string Name { get; set; }
    public required DateTime Date { get; set; }
    public required string Location { get; set; }
    public required ICollection<long> RefereeIds { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CompetitionLevel Level { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required FutsalTournamentType TournamentType { get; set; }
    
    public required string Description { get; set; }
    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }

    public long? FirstPlaceTeamId { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
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
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required FutsalTournamentType TournamentType { get; set; }

    public required string Description { get; set; }
    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }

    public FutsalTeamResponse? FirstPlaceTeam { get; set; }
    public FutsalTeamResponse? SecondPlaceTeam { get; set; }
    public FutsalTeamResponse? ThirdPlaceTeam { get; set; }
    public ICollection<FutsalTeamResponse>? Teams { get; set; }
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
            FirstPlaceTeamId = request.FirstPlaceTeamId,
            SecondPlaceTeamId = request.SecondPlaceTeamId,
            ThirdPlaceTeamId = request.ThirdPlaceTeamId
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
            TournamentType = entity.TournamentType,
            Description = entity.Description,
            RegistrationStart = entity.RegistrationStart,
            RegistrationEnd = entity.RegistrationEnd,
            EndDate = entity.EndDate,
            Rules = entity.Rules,
            Prizes = entity.Prizes,
            HeadRefereeName = entity.HeadRefereeName,
            Referees = entity.Referees,
            Schedule = entity.Schedule,
            FirstPlaceTeam = entity.FirstPlaceTeam?.ToResponse(),
            SecondPlaceTeam = entity.SecondPlaceTeam?.ToResponse(),
            ThirdPlaceTeam = entity.ThirdPlaceTeam?.ToResponse(),
            Teams = entity.Teams?.Select(t => t.ToResponse()).ToList(),
            Matches = entity.Matches?.Select(m => m.ToResponse()).ToList()
        };
    }
}
