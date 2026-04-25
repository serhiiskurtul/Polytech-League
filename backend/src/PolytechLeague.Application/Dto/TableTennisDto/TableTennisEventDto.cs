using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.TableTennis;
using PolytechLeague.Domain.Entities.TableTennis.Enums;
using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Application.Dto.ChessDto; // For EventPhotoDto if shared, otherwise adjust

namespace PolytechLeague.Application.Dto.TableTennisDto;

public class TableTennisEventRequest
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
    public required TableTennisTournamentType TournamentType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required TableTennisCategory Category { get; set; }

    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsTeam { get; set; }
    public bool IsModerated { get; set; }
    public bool IsPublic { get; set; }
    public long? SeasonId { get; set; }
}

public class TableTennisEventResponse
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
    public required TableTennisTournamentType TournamentType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required TableTennisCategory Category { get; set; }
    
    public PlayerDto? FirstPlace { get; set; }
    public PlayerDto? SecondPlace { get; set; }
    public PlayerDto? ThirdPlace { get; set; }

    public ICollection<TableTennisMatchResponse>? Matches { get; set; }
    public int ParticipantsCount { get; set; }
    public ICollection<TableTennisEventParticipantDto>? Participants { get; set; }

    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsTeam { get; set; }
    public bool IsModerated { get; set; }
    public long? SeasonId { get; set; }
}

public static class TableTennisEventDtoExtensions
{
    public static TableTennisEvent FromRequest(this TableTennisEventRequest request)
    {
        return new TableTennisEvent
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
            TournamentType = request.TournamentType,
            Category = request.Category,
            Rules = request.Rules,
            Prizes = request.Prizes,
            HeadRefereeName = request.HeadRefereeName,
            Referees = request.Referees,
            Schedule = request.Schedule,
            IsTeam = request.IsTeam,
            IsModerated = request.IsModerated,
            IsPublic = request.IsPublic,
            SeasonId = request.SeasonId
        };
    }

    public static TableTennisEventResponse ToResponse(this TableTennisEvent entity)
    {
        return new TableTennisEventResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Date = entity.Date,
            Location = entity.Location,
            Photos = entity.Photos?.Select(p => p.ToDto()).ToList(),
            RefereeIds = entity.RefereeIds,
            Level = entity.Level,
            Description = entity.Description,
            TournamentType = entity.TournamentType,
            Category = entity.Category,
            FirstPlace = entity.FirstPlace?.ToDto(),
            SecondPlace = entity.SecondPlace?.ToDto(),
            ThirdPlace = entity.ThirdPlace?.ToDto(),
            Matches = entity.Matches?.Select(m => m.ToResponse()).ToList(),
            ParticipantsCount = entity.Participants?.Count ?? 0,
            Participants = entity.Participants?.Select(p => p.ToDto()).ToList(),
            Rules = entity.Rules,
            Prizes = entity.Prizes,
            HeadRefereeName = entity.HeadRefereeName,
            Referees = entity.Referees,
            Schedule = entity.Schedule,
            IsTeam = entity.IsTeam,
            IsModerated = entity.IsModerated,
            SeasonId = entity.SeasonId
        };
    }
}
