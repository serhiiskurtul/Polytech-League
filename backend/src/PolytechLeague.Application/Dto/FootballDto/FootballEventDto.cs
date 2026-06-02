using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.Football;
using PolytechLeague.Domain.Entities.Football.Enums;
using PolytechLeague.Application.Dto.ChessDto;
using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Application.Dto.FootballDto;

public class FootballEventRequest
{
    public required string Name { get; set; }
    public required DateTime Date { get; set; }
    public required string Location { get; set; }
    public required ICollection<long> RefereeIds { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required CompetitionLevel Level { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FootballTournamentType? TournamentType { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EventCategory Category { get; set; } = EventCategory.Mixed;
    
    public string? Description { get; set; }

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
    public bool DisableNotifications { get; set; }
    public bool IsTeam { get; set; }

    public long? FirstPlaceTeamId { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public long? SeasonId { get; set; }
    public long? MvpId { get; set; }
}


public class FootballEventResponse
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
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FootballTournamentType? TournamentType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EventCategory Category { get; set; }

    public string? Description { get; set; }

    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }

    public FootballTeamResponse? FirstPlaceTeam { get; set; }
    public FootballTeamResponse? SecondPlaceTeam { get; set; }
    public FootballTeamResponse? ThirdPlaceTeam { get; set; }
    public ICollection<FootballTeamResponse>? Teams { get; set; }
    public ICollection<FootballMatchResponse>? Matches { get; set; }
    public ICollection<FootballEventParticipantDto>? Participants { get; set; }
    public long? SeasonId { get; set; }
    public bool IsModerated { get; set; }
    public bool IsPublic { get; set; }
    public bool DisableNotifications { get; set; }
    public bool IsTeam { get; set; }
    public long? MvpId { get; set; }
    public string? MvpName { get; set; }
}


public class FootballEventParticipantDto
{
    public long PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public string? GroupName { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RegistrationStatus Status { get; set; }
}

public static class FootballEventDtoExtensions
{
    public static FootballEvent FromRequest(this FootballEventRequest request)
    {
        return new FootballEvent
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
            DisableNotifications = request.DisableNotifications,
            IsTeam = request.IsTeam,
            FirstPlaceTeamId = request.FirstPlaceTeamId,
            SecondPlaceTeamId = request.SecondPlaceTeamId,
            ThirdPlaceTeamId = request.ThirdPlaceTeamId,
            SeasonId = request.SeasonId,
            Category = request.Category,
            MvpId = request.MvpId
        };
    }


    public static FootballEventResponse ToResponse(this FootballEvent entity)
    {
        return new FootballEventResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Date = entity.Date,
            Location = entity.Location,
            Photos = entity.Photos?.Select(p => p.ToDto()).ToList(),
            MainPhotoUrl = entity.MainPhotoUrl,
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
            Matches = entity.Matches?.Select(m => m.ToResponse()).ToList(),
            Participants = entity.Participants?.Select(p => new FootballEventParticipantDto
            {
                PlayerId = p.PlayerId,
                PlayerName = p.Player != null ? $"{p.Player.FirstName} {p.Player.LastName}" : null,
                GroupName = p.GroupName,
                Status = p.Status
            }).ToList(),
            SeasonId = entity.SeasonId,
            Category = entity.Category,
            IsModerated = entity.IsModerated,
            IsPublic = entity.IsPublic,
            DisableNotifications = entity.DisableNotifications,
            IsTeam = entity.IsTeam,
            MvpId = entity.MvpId,
            MvpName = entity.Mvp != null ? $"{entity.Mvp.FirstName} {entity.Mvp.LastName}" : null
        };
    }

}
