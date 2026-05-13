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
    
    public string? Description { get; set; }
    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TableTennisTournamentType? TournamentType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EventCategory Category { get; set; }


    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsTeam { get; set; }
    public bool IsModerated { get; set; }
    public bool IsPublic { get; set; }
    public long? SeasonId { get; set; }

    public long? FirstPlaceId { get; set; }
    public long? SecondPlaceId { get; set; }
    public long? ThirdPlaceId { get; set; }
    public long? FirstPlaceTeamId { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public long? MvpId { get; set; }
}


public class TableTennisTeamResponse
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public long EventId { get; set; }
    public ICollection<PlayerDto>? Players { get; set; }
    public int Points { get; set; }
    public int Rank { get; set; }
}

public class TableTennisEventResponse
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

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TableTennisTournamentType? TournamentType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EventCategory? Category { get; set; }

    
    public PlayerDto? FirstPlace { get; set; }
    public PlayerDto? SecondPlace { get; set; }
    public PlayerDto? ThirdPlace { get; set; }

    public TableTennisTeamResponse? FirstPlaceTeam { get; set; }
    public TableTennisTeamResponse? SecondPlaceTeam { get; set; }
    public TableTennisTeamResponse? ThirdPlaceTeam { get; set; }

    public ICollection<TableTennisMatchResponse>? Matches { get; set; }
    public int ParticipantsCount { get; set; }
    public ICollection<TableTennisEventParticipantDto>? Participants { get; set; }
    public ICollection<TableTennisTeamResponse>? Teams { get; set; }

    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsTeam { get; set; }
    public bool IsModerated { get; set; }
    public long? MvpId { get; set; }
    public string? MvpName { get; set; }
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
            SeasonId = request.SeasonId,
            FirstPlaceId = request.FirstPlaceId,
            SecondPlaceId = request.SecondPlaceId,
            ThirdPlaceId = request.ThirdPlaceId,
            FirstPlaceTeamId = request.FirstPlaceTeamId,
            SecondPlaceTeamId = request.SecondPlaceTeamId,
            ThirdPlaceTeamId = request.ThirdPlaceTeamId,
            MvpId = request.MvpId
        };

        };
    }

    public static TableTennisEventResponse ToResponse(this TableTennisEvent entity)
    {
        var response = new TableTennisEventResponse
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
            TournamentType = entity.TournamentType,
            Category = entity.Category,
            FirstPlaceTeam = entity.FirstPlaceTeam?.ToResponse(),
            SecondPlaceTeam = entity.SecondPlaceTeam?.ToResponse(),
            ThirdPlaceTeam = entity.ThirdPlaceTeam?.ToResponse(),
            Matches = entity.Matches?.Select(m => m.ToResponse()).ToList(),
            ParticipantsCount = entity.Participants?.Count ?? 0,
            Participants = entity.Participants?.Select(p => p.ToDto()).ToList(),
            Teams = entity.Teams?.Select(t => t.ToResponse()).ToList(),
            Rules = entity.Rules,
            Prizes = entity.Prizes,
            HeadRefereeName = entity.HeadRefereeName,
            Referees = entity.Referees,
            Schedule = entity.Schedule,
            IsTeam = entity.IsTeam,
            IsModerated = entity.IsModerated,
            MvpId = entity.MvpId,
            MvpName = entity.Mvp != null ? $"{entity.Mvp.FirstName} {entity.Mvp.LastName}" : null,
            SeasonId = entity.SeasonId

        };

        if (entity.Participants != null)
        {
            if (entity.FirstPlaceId != null)
            {
                var p = entity.Participants.FirstOrDefault(x => x.PlayerId == entity.FirstPlaceId);
                if (p != null) response.FirstPlace = p.ToWinnerDto();
                else if (entity.FirstPlace != null) response.FirstPlace = entity.FirstPlace.ToDto();
            }
            if (entity.SecondPlaceId != null)
            {
                var p = entity.Participants.FirstOrDefault(x => x.PlayerId == entity.SecondPlaceId);
                if (p != null) response.SecondPlace = p.ToWinnerDto();
                else if (entity.SecondPlace != null) response.SecondPlace = entity.SecondPlace.ToDto();
            }
            if (entity.ThirdPlaceId != null)
            {
                var p = entity.Participants.FirstOrDefault(x => x.PlayerId == entity.ThirdPlaceId);
                if (p != null) response.ThirdPlace = p.ToWinnerDto();
                else if (entity.ThirdPlace != null) response.ThirdPlace = entity.ThirdPlace.ToDto();
            }
        }
        else
        {
            response.FirstPlace = entity.FirstPlace?.ToDto();
            response.SecondPlace = entity.SecondPlace?.ToDto();
            response.ThirdPlace = entity.ThirdPlace?.ToDto();
        }

        return response;
    }

    private static PlayerDto ToWinnerDto(this TableTennisEventParticipant participant)
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

    public static TableTennisTeamResponse ToResponse(this TableTennisTeam entity)
    {
        return new TableTennisTeamResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            EventId = entity.EventId,
            Players = entity.Players?.Select(p => p.ToDto()).ToList(),
            Points = entity.Points,
            Rank = entity.Rank
        };
    }
}
