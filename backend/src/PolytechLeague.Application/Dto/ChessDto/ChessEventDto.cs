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
    
    public string? Description { get; set; }
    public string? TimeControl { get; set; }
    public int ParticipantsCount { get; set; }
    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChessSystem? System { get; set; }

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
    public bool DisableNotifications { get; set; }

    public long? FirstPlaceId { get; set; }
    public long? SecondPlaceId { get; set; }
    public long? ThirdPlaceId { get; set; }
    
    public long? FirstPlaceTeamId { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public long? MvpId { get; set; }
    public long? SeasonId { get; set; }
}


public class ChessEventResponse
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
    public string? TimeControl { get; set; }
    public int ParticipantsCount { get; set; }
    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChessSystem? System { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EventCategory Category { get; set; }

    public required ICollection<ChessGameDto> Games { get; set; }
    public required ICollection<ChessEventParticipantDto> Participants { get; set; }
    public ICollection<ChessTeamResponse>? Teams { get; set; }
    
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsTeam { get; set; }
    public bool IsModerated { get; set; }
    public bool IsPublic { get; set; }
    public bool DisableNotifications { get; set; }

    public PlayerDto? FirstPlace { get; set; }
    public PlayerDto? SecondPlace { get; set; }
    public PlayerDto? ThirdPlace { get; set; }

    public ChessTeamResponse? FirstPlaceTeam { get; set; }
    public ChessTeamResponse? SecondPlaceTeam { get; set; }
    public ChessTeamResponse? ThirdPlaceTeam { get; set; }
    public long? MvpId { get; set; }
    public string? MvpName { get; set; }
    public long? SeasonId { get; set; }
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
            RegistrationStart = chessEventRequest.RegistrationStart,
            RegistrationEnd = chessEventRequest.RegistrationEnd,
            EndDate = chessEventRequest.EndDate,
            System = chessEventRequest.System,
            Rules = chessEventRequest.Rules,
            Prizes = chessEventRequest.Prizes,
            HeadRefereeName = chessEventRequest.HeadRefereeName,
            Referees = chessEventRequest.Referees,
            Schedule = chessEventRequest.Schedule,
            IsTeam = chessEventRequest.IsTeam,
            IsModerated = chessEventRequest.IsModerated,
            IsPublic = chessEventRequest.IsPublic,
            DisableNotifications = chessEventRequest.DisableNotifications,
            FirstPlaceId = chessEventRequest.FirstPlaceId,
            SecondPlaceId = chessEventRequest.SecondPlaceId,
            ThirdPlaceId = chessEventRequest.ThirdPlaceId,
            FirstPlaceTeamId = chessEventRequest.FirstPlaceTeamId,
            SecondPlaceTeamId = chessEventRequest.SecondPlaceTeamId,
            ThirdPlaceTeamId = chessEventRequest.ThirdPlaceTeamId,
            MvpId = chessEventRequest.MvpId,
            SeasonId = chessEventRequest.SeasonId,

            Category = chessEventRequest.Category
        };
    }

    public static ChessEventResponse ToResponse(this ChessEvent chessEvent)
    {
        var response = new ChessEventResponse
        {
            Id = chessEvent.Id,
            Name = chessEvent.Name,
            Date = chessEvent.Date,
            Location = chessEvent.Location,
            Photos = chessEvent.Photos?.Select(p => p.ToDto()).ToList(),
            MainPhotoUrl = chessEvent.MainPhotoUrl,
            RefereeIds = chessEvent.RefereeIds,
            Level = chessEvent.Level,
            Description = chessEvent.Description,
            TimeControl = chessEvent.TimeControl,
            ParticipantsCount = chessEvent.ParticipantsCount,
            RegistrationStart = chessEvent.RegistrationStart,
            RegistrationEnd = chessEvent.RegistrationEnd,
            EndDate = chessEvent.EndDate,
            System = chessEvent.System,
            Games = chessEvent.Games?.Select(g => g.ToDto(chessEvent.Players)).ToList() ?? [],
            Participants = chessEvent.Players?.Select(p => p.ToDto()).ToList() ?? [],
            Teams = chessEvent.Teams?.Select(t => t.ToResponse()).ToList(),
            Rules = chessEvent.Rules,
            Prizes = chessEvent.Prizes,
            HeadRefereeName = chessEvent.HeadRefereeName,
            Referees = chessEvent.Referees,
            Schedule = chessEvent.Schedule,
            IsTeam = chessEvent.IsTeam,
            IsModerated = chessEvent.IsModerated,
            IsPublic = chessEvent.IsPublic,
            DisableNotifications = chessEvent.DisableNotifications,
            SeasonId = chessEvent.SeasonId,
            Category = chessEvent.Category,
            FirstPlaceTeam = chessEvent.FirstPlaceTeam?.ToResponse(),
            SecondPlaceTeam = chessEvent.SecondPlaceTeam?.ToResponse(),
            ThirdPlaceTeam = chessEvent.ThirdPlaceTeam?.ToResponse(),
            MvpId = chessEvent.MvpId,
            MvpName = chessEvent.Mvp != null ? $"{chessEvent.Mvp.FirstName} {chessEvent.Mvp.LastName}" : null
        };


        if (chessEvent.Players != null)
        {
            if (chessEvent.FirstPlaceId != null)
            {
                var p = chessEvent.Players.FirstOrDefault(x => x.PlayerId == chessEvent.FirstPlaceId);
                if (p != null) response.FirstPlace = p.ToWinnerDto();
                else if (chessEvent.FirstPlace != null) response.FirstPlace = chessEvent.FirstPlace.ToDto();
            }
            if (chessEvent.SecondPlaceId != null)
            {
                var p = chessEvent.Players.FirstOrDefault(x => x.PlayerId == chessEvent.SecondPlaceId);
                if (p != null) response.SecondPlace = p.ToWinnerDto();
                else if (chessEvent.SecondPlace != null) response.SecondPlace = chessEvent.SecondPlace.ToDto();
            }
            if (chessEvent.ThirdPlaceId != null)
            {
                var p = chessEvent.Players.FirstOrDefault(x => x.PlayerId == chessEvent.ThirdPlaceId);
                if (p != null) response.ThirdPlace = p.ToWinnerDto();
                else if (chessEvent.ThirdPlace != null) response.ThirdPlace = chessEvent.ThirdPlace.ToDto();
            }
        }
        else
        {
            response.FirstPlace = chessEvent.FirstPlace?.ToDto();
            response.SecondPlace = chessEvent.SecondPlace?.ToDto();
            response.ThirdPlace = chessEvent.ThirdPlace?.ToDto();
        }

        return response;
    }

    private static PlayerDto ToWinnerDto(this ChessEventParticipant participant)
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
