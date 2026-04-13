using PolytechLeague.Application.Dto.GeneralDto;
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
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsTeam { get; set; }

    public long? FirstPlaceTeamId { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
}

public class BadmintonTeamDto
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public long Player1Id { get; set; }
    public PlayerDto? Player1 { get; set; }
    public long Player2Id { get; set; }
    public PlayerDto? Player2 { get; set; }
}

public class BadmintonTeamRequest
{
    public string? Name { get; set; }
    public long Player1Id { get; set; }
    public long Player2Id { get; set; }
    public long EventId { get; set; }
    public ICollection<long>? PlayerIds { get; set; } // For compatibility with TeamSportsTournamentView
}

public class BadmintonTeamResponse
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public long Player1Id { get; set; }
    public PlayerDto? Player1 { get; set; }
    public long Player2Id { get; set; }
    public PlayerDto? Player2 { get; set; }
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
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsTeam { get; set; }

    public PlayerDto? FirstPlace { get; set; }
    public PlayerDto? SecondPlace { get; set; }
    public PlayerDto? ThirdPlace { get; set; }
    
    public BadmintonTeamDto? FirstPlaceTeam { get; set; }
    public BadmintonTeamDto? SecondPlaceTeam { get; set; }
    public BadmintonTeamDto? ThirdPlaceTeam { get; set; }
    public ICollection<BadmintonTeamDto>? Teams { get; set; }
    public ICollection<BadmintonMatchResponse>? Matches { get; set; }
    public ICollection<BadmintonEventParticipantDto>? Players { get; set; }
}

public class BadmintonEventParticipantDto
{
    public long PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public double Points { get; set; }
    public int Rank { get; set; }
    public long? UserId { get; set; }
}

public class BadmintonMatchResponse
{
    public long Id { get; set; }
    public long? Player1Id { get; set; }
    public PlayerDto? Player1 { get; set; }
    public long? Player2Id { get; set; }
    public PlayerDto? Player2 { get; set; }

    public long? Team1Id { get; set; }
    public BadmintonTeamDto? Team1 { get; set; }
    public long? Team2Id { get; set; }
    public BadmintonTeamDto? Team2 { get; set; }

    public long? WinnerPlayerId { get; set; }
    public PlayerDto? WinnerPlayer { get; set; }
    public long? WinnerTeamId { get; set; }
    public BadmintonTeamDto? WinnerTeam { get; set; }
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
            Description = request.Description,
            Rules = request.Rules,
            Prizes = request.Prizes,
            HeadRefereeName = request.HeadRefereeName,
            Referees = request.Referees,
            Schedule = request.Schedule,
            IsTeam = request.IsTeam
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
            Rules = entity.Rules,
            Prizes = entity.Prizes,
            HeadRefereeName = entity.HeadRefereeName,
            Referees = entity.Referees,
            Schedule = entity.Schedule,
            IsTeam = entity.IsTeam,
            FirstPlaceTeam = entity.FirstPlaceTeam?.ToDto(),
            SecondPlaceTeam = entity.SecondPlaceTeam?.ToDto(),
            ThirdPlaceTeam = entity.ThirdPlaceTeam?.ToDto(),
            Teams = entity.Teams?.Select(t => t.ToDto()).ToList(),
            Matches = entity.Matches?.Select(m => m.ToResponse()).ToList(),
            Players = entity.Participants?.Select(p => p.ToDto()).ToList()
        };
    }

    public static BadmintonEventParticipantDto ToDto(this BadmintonEventParticipant entity)
    {
        return new BadmintonEventParticipantDto
        {
            PlayerId = entity.PlayerId,
            PlayerName = entity.Player != null ? $"{entity.Player.FirstName} {entity.Player.LastName}" : null,
            Points = entity.Points,
            Rank = entity.Rank,
            UserId = entity.Player?.UserId
        };
    }

    public static BadmintonMatchResponse ToResponse(this BadmintonMatch entity)
    {
        return new BadmintonMatchResponse
        {
            Id = entity.Id,
            Player1Id = entity.Player1Id,
            Player1 = entity.Player1?.ToDto(),
            Player2Id = entity.Player2Id,
            Player2 = entity.Player2?.ToDto(),
            Team1Id = entity.Team1Id,
            Team1 = entity.Team1?.ToDto(),
            Team2Id = entity.Team2Id,
            Team2 = entity.Team2?.ToDto(),
            WinnerPlayerId = entity.WinnerPlayerId,
            WinnerPlayer = entity.WinnerPlayer?.ToDto(),
            WinnerTeamId = entity.WinnerTeamId,
            WinnerTeam = entity.WinnerTeam?.ToDto(),
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

    public static BadmintonTeam ToEntity(this BadmintonTeamRequest request)
    {
        return new BadmintonTeam
        {
            Name = request.Name,
            Player1Id = request.Player1Id,
            Player2Id = request.Player2Id,
            EventId = request.EventId,
            PlayerIds = request.PlayerIds
        };
    }

    public static BadmintonTeamResponse ToResponse(this BadmintonTeam entity)
    {
        return new BadmintonTeamResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Player1Id = entity.Player1Id,
            Player1 = entity.Player1?.ToDto(),
            Player2Id = entity.Player2Id,
            Player2 = entity.Player2?.ToDto()
        };
    }

    public static BadmintonTeamDto ToDto(this BadmintonTeam entity)
    {
        return new BadmintonTeamDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Player1Id = entity.Player1Id,
            Player1 = entity.Player1?.ToDto(),
            Player2Id = entity.Player2Id,
            Player2 = entity.Player2?.ToDto()
        };
    }
}
