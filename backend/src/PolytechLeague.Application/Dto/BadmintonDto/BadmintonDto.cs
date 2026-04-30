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
    public BadmintonTournamentType? TournamentType { get; set; }
    
    public string? Description { get; set; }

    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsTeam { get; set; }
    public bool IsModerated { get; set; }
    public bool IsPublic { get; set; }

    public long? FirstPlaceTeamId { get; set; }
    public long? SecondPlaceTeamId { get; set; }
    public long? ThirdPlaceTeamId { get; set; }
    public long? SeasonId { get; set; }
}

public class BadmintonTeamDto
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public long? Player1Id { get; set; }
    public PlayerDto? Player1 { get; set; }
    public long? Player2Id { get; set; }
    public PlayerDto? Player2 { get; set; }
}

public class BadmintonTeamRequest
{
    public string? Name { get; set; }
    public long? Player1Id { get; set; }
    public long? Player2Id { get; set; }
    public long EventId { get; set; }
    public ICollection<long>? PlayerIds { get; set; } // For compatibility with TeamSportsTournamentView
}

public class BadmintonTeamResponse
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public long? Player1Id { get; set; }
    public PlayerDto? Player1 { get; set; }
    public long? Player2Id { get; set; }
    public PlayerDto? Player2 { get; set; }
}

public class BadmintonEventResponse
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
    public BadmintonTournamentType? TournamentType { get; set; }

    public string? Description { get; set; }

    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Rules { get; set; }
    public string? Prizes { get; set; }
    public string? HeadRefereeName { get; set; }
    public string? Referees { get; set; }
    public string? Schedule { get; set; }
    public bool IsTeam { get; set; }
    public bool IsModerated { get; set; }

    public PlayerDto? FirstPlace { get; set; }
    public PlayerDto? SecondPlace { get; set; }
    public PlayerDto? ThirdPlace { get; set; }
    
    public BadmintonTeamDto? FirstPlaceTeam { get; set; }
    public BadmintonTeamDto? SecondPlaceTeam { get; set; }
    public BadmintonTeamDto? ThirdPlaceTeam { get; set; }
    public ICollection<BadmintonTeamDto>? Teams { get; set; }
    public ICollection<BadmintonMatchResponse>? Matches { get; set; }
    public ICollection<BadmintonEventParticipantDto>? Participants { get; set; }
    public long? SeasonId { get; set; }
}

public class BadmintonMatchRequest
{
    public long EventId { get; set; }
    public long? Player1Id { get; set; }
    public long? Player2Id { get; set; }
    public long? HomeTeamId { get; set; }
    public long? AwayTeamId { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public long? WinnerPlayerId { get; set; }
    public long? WinnerTeamId { get; set; }
    public string? Stage { get; set; }
    public int CourtNumber { get; set; }
    public int SetsToWin { get; set; }
    public ICollection<long>? HomeRosterIds { get; set; }
    public ICollection<long>? AwayRosterIds { get; set; }
}

public class BadmintonMatchResponse
{
    public long Id { get; set; }
    public long? Player1Id { get; set; }
    public PlayerDto? Player1 { get; set; }
    public long? Player2Id { get; set; }
    public PlayerDto? Player2 { get; set; }

    public long? HomeTeamId { get; set; }
    public long? AwayTeamId { get; set; }

    public long? WinnerPlayerId { get; set; }
    public PlayerDto? WinnerPlayer { get; set; }
    public long? WinnerTeamId { get; set; }
    public BadmintonTeamDto? WinnerTeam { get; set; }
    public string? Stage { get; set; }
    public int CourtNumber { get; set; }
    public int SetsToWin { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public ICollection<BadmintonSetResponse>? Sets { get; set; }

    public BadmintonTeamDto? HomeTeam { get; set; }
    public BadmintonTeamDto? AwayTeam { get; set; }

    public string? HomeTeamName { get; set; }
    public string? AwayTeamName { get; set; }
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
            RegistrationStart = request.RegistrationStart,
            RegistrationEnd = request.RegistrationEnd,
            EndDate = request.EndDate,
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

    public static BadmintonEventResponse ToResponse(this BadmintonEvent entity)
    {
        return new BadmintonEventResponse
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
            IsTeam = entity.IsTeam,
            IsModerated = entity.IsModerated,
            FirstPlace = entity.FirstPlace?.ToDto(),
            SecondPlace = entity.SecondPlace?.ToDto(),
            ThirdPlace = entity.ThirdPlace?.ToDto(),
            FirstPlaceTeam = entity.FirstPlaceTeam?.ToDto(),
            SecondPlaceTeam = entity.SecondPlaceTeam?.ToDto(),
            ThirdPlaceTeam = entity.ThirdPlaceTeam?.ToDto(),
            Teams = entity.Teams?.Select(t => t.ToDto()).ToList(),
            Matches = entity.Matches?.Select(m => m.ToResponse()).ToList(),
            Participants = entity.Participants?.Select(p => p.ToDto()).ToList(),
            SeasonId = entity.SeasonId
        };
    }

    public static BadmintonMatch FromRequest(this BadmintonMatchRequest request)
    {
        return new BadmintonMatch
        {
            EventId = request.EventId,
            Player1Id = request.Player1Id,
            Player2Id = request.Player2Id,
            HomeTeamId = request.HomeTeamId,
            AwayTeamId = request.AwayTeamId,
            HomeScore = request.HomeScore,
            AwayScore = request.AwayScore,
            WinnerPlayerId = request.WinnerPlayerId,
            WinnerTeamId = request.WinnerTeamId,
            Stage = request.Stage,
            CourtNumber = request.CourtNumber,
            SetsToWin = request.SetsToWin,
            HomeRosterIds = request.HomeRosterIds,
            AwayRosterIds = request.AwayRosterIds
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
            HomeTeamId = entity.HomeTeamId,
            AwayTeamId = entity.AwayTeamId,
            HomeScore = entity.HomeScore,
            AwayScore = entity.AwayScore,
            WinnerPlayerId = entity.WinnerPlayerId,
            WinnerPlayer = entity.WinnerPlayer?.ToDto(),
            WinnerTeamId = entity.WinnerTeamId,
            WinnerTeam = entity.WinnerTeam?.ToDto(),
            Stage = entity.Stage,
            CourtNumber = entity.CourtNumber,
            SetsToWin = entity.SetsToWin,
            Sets = entity.Sets?.Select(s => s.ToResponse()).ToList(),
            HomeTeam = entity.HomeTeam?.ToDto(),
            AwayTeam = entity.AwayTeam?.ToDto(),
            HomeTeamName = entity.HomeTeam?.Name,
            AwayTeamName = entity.AwayTeam?.Name
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
