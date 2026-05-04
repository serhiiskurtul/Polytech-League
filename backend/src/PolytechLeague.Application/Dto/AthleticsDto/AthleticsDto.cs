using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Athletics;
using PolytechLeague.Application.Dto.GeneralDto;

namespace PolytechLeague.Application.Dto.AthleticsDto;

public class AthleticsEventRequest
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
    public EventCategory Category { get; set; } = EventCategory.Mixed;

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

public class AthleticsEventResponse
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
    public EventCategory Category { get; set; }

    public required ICollection<AthleticsResultDto> Results { get; set; }
    public required ICollection<AthleticsEventParticipantDto> Participants { get; set; }
    
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

public class AthleticsResultDto
{
    public long Id { get; set; }
    public required string DisciplineName { get; set; }
    public long? PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? TeamName { get; set; }
    public string? Score { get; set; }
    public int Rank { get; set; }
}

public class AthleticsEventParticipantDto
{
    public long PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? GroupName { get; set; }
}

public static class AthleticsDtoExtensions
{
    public static AthleticsEvent FromRequest(this AthleticsEventRequest request)
    {
        return new AthleticsEvent
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

    public static AthleticsEventResponse ToResponse(this AthleticsEvent entity)
    {
        return new AthleticsEventResponse
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
            Category = entity.Category,
            Rules = entity.Rules,
            Prizes = entity.Prizes,
            HeadRefereeName = entity.HeadRefereeName,
            Referees = entity.Referees,
            Schedule = entity.Schedule,
            IsTeam = entity.IsTeam,
            IsModerated = entity.IsModerated,
            IsPublic = entity.IsPublic,
            SeasonId = entity.SeasonId,
            Results = entity.Results?.Select(r => r.ToDto()).ToList() ?? [],
            Participants = entity.Participants?.Select(p => p.ToDto()).ToList() ?? []
        };
    }

    public static AthleticsResultDto ToDto(this AthleticsResult result)
    {
        return new AthleticsResultDto
        {
            Id = result.Id,
            DisciplineName = result.DisciplineName,
            PlayerId = result.PlayerId,
            PlayerName = result.Player != null ? $"{result.Player.FirstName} {result.Player.LastName}" : null,
            TeamName = result.TeamName,
            Score = result.Score,
            Rank = result.Rank
        };
    }

    public static AthleticsEventParticipantDto ToDto(this AthleticsEventParticipant participant)
    {
        return new AthleticsEventParticipantDto
        {
            PlayerId = participant.PlayerId,
            PlayerName = participant.PlayerFirstName != null ? $"{participant.PlayerFirstName} {participant.PlayerLastName}" : (participant.Player != null ? $"{participant.Player.FirstName} {participant.Player.LastName}" : "Unknown"),
            GroupName = participant.GroupName ?? participant.Player?.Group
        };
    }
}
