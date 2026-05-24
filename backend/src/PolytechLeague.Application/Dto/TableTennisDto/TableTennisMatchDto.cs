using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Entities.TableTennis;
using PolytechLeague.Application.Dto.GeneralDto;


namespace PolytechLeague.Application.Dto.TableTennisDto;

public class TableTennisSetDto
{
    public long Id { get; set; }
    public int SetNumber { get; set; }
    public int Player1Score { get; set; }
    public int Player2Score { get; set; }
}

public class TableTennisMatchRequest
{
    public long TournamentId { get; set; }
    public long? Player1Id { get; set; }
    public long? Player2Id { get; set; }
    public long? HomeTeamId { get; set; }
    public long? AwayTeamId { get; set; }
    public long? WinnerId { get; set; }
    public long? WinnerTeamId { get; set; }
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
    public string? Stage { get; set; }
    public int TableNumber { get; set; }
    public int SetsToWin { get; set; }
}

public class TableTennisMatchResponse
{
    public long Id { get; set; }
    public long TournamentId { get; set; }
    public long? Player1Id { get; set; }
    public PlayerDto? Player1 { get; set; }
    public long? Player2Id { get; set; }
    public PlayerDto? Player2 { get; set; }
    public long? HomeTeamId { get; set; }
    public TableTennisTeamResponse? HomeTeam { get; set; }
    public long? AwayTeamId { get; set; }
    public TableTennisTeamResponse? AwayTeam { get; set; }
    public long? WinnerId { get; set; }
    public long? WinnerTeamId { get; set; }
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
    public string? Stage { get; set; }
    public int TableNumber { get; set; }
    public int SetsToWin { get; set; }
    public ICollection<TableTennisSetDto>? Sets { get; set; }
}

public static class TableTennisMatchDtoExtensions
{
    public static TableTennisMatch FromRequest(this TableTennisMatchRequest request)
    {
        return new TableTennisMatch
        {
            TournamentId = request.TournamentId,
            Player1Id = request.Player1Id,
            Player2Id = request.Player2Id,
            HomeTeamId = request.HomeTeamId,
            AwayTeamId = request.AwayTeamId,
            WinnerId = request.WinnerId,
            WinnerTeamId = request.WinnerTeamId,
            HomeTeamScore = request.HomeTeamScore,
            AwayTeamScore = request.AwayTeamScore,
            Stage = request.Stage,
            TableNumber = request.TableNumber,
            SetsToWin = request.SetsToWin
        };
    }

    private static PlayerDto? ToPlayerDto(Player? player, long? playerId, IEnumerable<TableTennisEventParticipant>? participants)
    {
        if (playerId == null) return null;
        var participant = participants?.FirstOrDefault(p => p.PlayerId == playerId);
        if (participant != null)
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
        return player?.ToDto();
    }

    public static TableTennisMatchResponse ToResponse(this TableTennisMatch entity, IEnumerable<TableTennisEventParticipant>? participants = null)
    {
        return new TableTennisMatchResponse
        {
            Id = entity.Id,
            TournamentId = entity.TournamentId,
            Player1Id = entity.Player1Id,
            Player1 = ToPlayerDto(entity.Player1, entity.Player1Id, participants),
            Player2Id = entity.Player2Id,
            Player2 = ToPlayerDto(entity.Player2, entity.Player2Id, participants),
            HomeTeamId = entity.HomeTeamId,
            HomeTeam = entity.HomeTeam?.ToResponse(),
            AwayTeamId = entity.AwayTeamId,
            AwayTeam = entity.AwayTeam?.ToResponse(),
            WinnerId = entity.WinnerId,
            WinnerTeamId = entity.WinnerTeamId,
            HomeTeamScore = entity.HomeTeamScore,
            AwayTeamScore = entity.AwayTeamScore,
            Stage = entity.Stage,
            TableNumber = entity.TableNumber,
            SetsToWin = entity.SetsToWin,
            Sets = entity.Sets?.Select(s => s.ToDto()).ToList()
        };
    }


    public static TableTennisSetDto ToDto(this TableTennisSet entity)
    {
        return new TableTennisSetDto
        {
            Id = entity.Id,
            SetNumber = entity.SetNumber,
            Player1Score = entity.Player1Score,
            Player2Score = entity.Player2Score
        };
    }
}
