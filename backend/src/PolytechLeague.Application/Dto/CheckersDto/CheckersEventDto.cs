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

    public string? Description { get; set; }
    public string? TimeControl { get; set; }
    public int ParticipantsCount { get; set; }
    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CheckersSystem? System { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CheckersVariant Variant { get; set; }

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

    public long? FirstPlaceId { get; set; }
    public long? SecondPlaceId { get; set; }
    public long? ThirdPlaceId { get; set; }

    public long? FirstPlaceTeamId { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public long? SeasonId { get; set; }
}

public class CheckersEventResponse
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
    public CheckersSystem? System { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CheckersVariant? Variant { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EventCategory Category { get; set; }


    public required ICollection<CheckersGameDto> Games { get; set; }
    public required ICollection<CheckersEventParticipantDto> Participants { get; set; }
    public ICollection<CheckersTeamResponse>? Teams { get; set; }

    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsTeam { get; set; }
    public bool IsModerated { get; set; }
    public bool IsPublic { get; set; }

    public PlayerDto? FirstPlace { get; set; }
    public PlayerDto? SecondPlace { get; set; }
    public PlayerDto? ThirdPlace { get; set; }

    public CheckersTeamResponse? FirstPlaceTeam { get; set; }
    public CheckersTeamResponse? SecondPlaceTeam { get; set; }
    public CheckersTeamResponse? ThirdPlaceTeam { get; set; }
    public long? SeasonId { get; set; }
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
            RegistrationStart = checkersEventRequest.RegistrationStart,
            RegistrationEnd = checkersEventRequest.RegistrationEnd,
            EndDate = checkersEventRequest.EndDate,
            System = checkersEventRequest.System,
            Variant = checkersEventRequest.Variant,
            Rules = checkersEventRequest.Rules,
            Prizes = checkersEventRequest.Prizes,
            HeadRefereeName = checkersEventRequest.HeadRefereeName,
            Referees = checkersEventRequest.Referees,
            Schedule = checkersEventRequest.Schedule,
            IsTeam = checkersEventRequest.IsTeam,
            IsModerated = checkersEventRequest.IsModerated,
            IsPublic = checkersEventRequest.IsPublic,
            FirstPlaceId = checkersEventRequest.FirstPlaceId,
            SecondPlaceId = checkersEventRequest.SecondPlaceId,
            ThirdPlaceId = checkersEventRequest.ThirdPlaceId,
            FirstPlaceTeamId = checkersEventRequest.FirstPlaceTeamId,
            SecondPlaceTeamId = checkersEventRequest.SecondPlaceTeamId,
            ThirdPlaceTeamId = checkersEventRequest.ThirdPlaceTeamId,
            SeasonId = checkersEventRequest.SeasonId,
            Category = checkersEventRequest.Category
        };
    }

    public static CheckersEventResponse ToResponse(this CheckersEvent checkersEvent)
    {
        var response = new CheckersEventResponse
        {
            Id = checkersEvent.Id,
            Name = checkersEvent.Name,
            Date = checkersEvent.Date,
            Location = checkersEvent.Location,
            Photos = checkersEvent.Photos?.Select(p => p.ToDto()).ToList(),
            MainPhotoUrl = checkersEvent.MainPhotoUrl,
            RefereeIds = checkersEvent.RefereeIds,
            Level = checkersEvent.Level,
            Description = checkersEvent.Description,
            TimeControl = checkersEvent.TimeControl,
            ParticipantsCount = checkersEvent.ParticipantsCount,
            RegistrationStart = checkersEvent.RegistrationStart,
            RegistrationEnd = checkersEvent.RegistrationEnd,
            EndDate = checkersEvent.EndDate,
            System = checkersEvent.System,
            Variant = checkersEvent.Variant,
            Games = checkersEvent.Games?.Select(g => g.ToDto()).ToList() ?? [],
            Participants = checkersEvent.Players?.Select(p => p.ToDto()).ToList() ?? [],
            Teams = checkersEvent.Teams?.Select(t => t.ToResponse()).ToList(),
            Rules = checkersEvent.Rules,
            Prizes = checkersEvent.Prizes,
            HeadRefereeName = checkersEvent.HeadRefereeName,
            Referees = checkersEvent.Referees,
            Schedule = checkersEvent.Schedule,
            IsTeam = checkersEvent.IsTeam,
            IsModerated = checkersEvent.IsModerated,
            IsPublic = checkersEvent.IsPublic,
            SeasonId = checkersEvent.SeasonId,
            Category = checkersEvent.Category,
            FirstPlaceTeam = checkersEvent.FirstPlaceTeam?.ToResponse(),
            SecondPlaceTeam = checkersEvent.SecondPlaceTeam?.ToResponse(),
            ThirdPlaceTeam = checkersEvent.ThirdPlaceTeam?.ToResponse()
        };

        if (checkersEvent.Players != null)
        {
            if (checkersEvent.FirstPlaceId != null)
            {
                var p = checkersEvent.Players.FirstOrDefault(x => x.PlayerId == checkersEvent.FirstPlaceId);
                if (p != null) response.FirstPlace = p.ToWinnerDto();
                else if (checkersEvent.FirstPlace != null) response.FirstPlace = checkersEvent.FirstPlace.ToDto();
            }
            if (checkersEvent.SecondPlaceId != null)
            {
                var p = checkersEvent.Players.FirstOrDefault(x => x.PlayerId == checkersEvent.SecondPlaceId);
                if (p != null) response.SecondPlace = p.ToWinnerDto();
                else if (checkersEvent.SecondPlace != null) response.SecondPlace = checkersEvent.SecondPlace.ToDto();
            }
            if (checkersEvent.ThirdPlaceId != null)
            {
                var p = checkersEvent.Players.FirstOrDefault(x => x.PlayerId == checkersEvent.ThirdPlaceId);
                if (p != null) response.ThirdPlace = p.ToWinnerDto();
                else if (checkersEvent.ThirdPlace != null) response.ThirdPlace = checkersEvent.ThirdPlace.ToDto();
            }
        }
        else
        {
            response.FirstPlace = checkersEvent.FirstPlace?.ToDto();
            response.SecondPlace = checkersEvent.SecondPlace?.ToDto();
            response.ThirdPlace = checkersEvent.ThirdPlace?.ToDto();
        }

        return response;
    }

    private static PlayerDto ToWinnerDto(this CheckersEventParticipant participant)
    {
        return new PlayerDto
        {
            Id = participant.PlayerId,
            FirstName = participant.PlayerFirstName ?? participant.Player?.FirstName ?? "Unknown",
            LastName = participant.PlayerLastName ?? participant.Player?.LastName ?? "",
            GroupName = participant.GroupName ?? participant.Player?.Group ?? "Unknown",
            PhotoUrl = participant.Player?.PhotoUrl
        };
    }
}
