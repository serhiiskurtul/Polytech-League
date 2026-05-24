using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Application.Dto.CheckersDto;

public class CheckersGameDto
{
    public long Id { get; set; }
    public long TournamentId { get; set; }

    public long? WhitePlayerId { get; set; }
    public string? WhitePlayerName { get; set; }
    public long? WhiteUserId { get; set; }

    public long? BlackPlayerId { get; set; }
    public string? BlackPlayerName { get; set; }
    public long? BlackUserId { get; set; }

    public long? WhiteTeamId { get; set; }
    public CheckersTeamResponse? WhiteTeam { get; set; }
    public long? BlackTeamId { get; set; }
    public CheckersTeamResponse? BlackTeam { get; set; }
    public double? WhiteTeamScore { get; set; }
    public double? BlackTeamScore { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GameResult Result { get; set; }

    public int Round { get; set; }

    public DateTime ScheduledAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}


public static class CheckersGameDtoExtensions
{
    public static CheckersGameDto ToDto(this CheckersGame game)
    {
        string? whiteName = null;
        if (game.WhitePlayer != null)
        {
            whiteName = (game.WhitePlayer.PlayerFirstName != null || game.WhitePlayer.PlayerLastName != null)
                ? $"{game.WhitePlayer.PlayerFirstName} {game.WhitePlayer.PlayerLastName}".Trim()
                : (game.WhitePlayer.Player != null ? $"{game.WhitePlayer.Player.FirstName} {game.WhitePlayer.Player.LastName}".Trim() : null);
        }

        string? blackName = null;
        if (game.BlackPlayer != null)
        {
            blackName = (game.BlackPlayer.PlayerFirstName != null || game.BlackPlayer.PlayerLastName != null)
                ? $"{game.BlackPlayer.PlayerFirstName} {game.BlackPlayer.PlayerLastName}".Trim()
                : (game.BlackPlayer.Player != null ? $"{game.BlackPlayer.Player.FirstName} {game.BlackPlayer.Player.LastName}".Trim() : null);
        }

        return new CheckersGameDto
        {
            Id = game.Id,
            TournamentId = game.TournamentId,
            WhitePlayerId = game.WhitePlayerId,
            WhitePlayerName = whiteName,
            WhiteUserId = game.WhitePlayer?.Player?.UserId,
            BlackPlayerId = game.BlackPlayerId,
            BlackPlayerName = blackName,
            BlackUserId = game.BlackPlayer?.Player?.UserId,
            WhiteTeamId = game.WhiteTeamId,
            WhiteTeam = game.WhiteTeam?.ToResponse(),
            BlackTeamId = game.BlackTeamId,
            BlackTeam = game.BlackTeam?.ToResponse(),
            WhiteTeamScore = game.WhiteTeamScore,
            BlackTeamScore = game.BlackTeamScore,
            Result = game.Result,
            Round = game.Round,
            ScheduledAt = game.ScheduledAt,
            FinishedAt = game.FinishedAt
        };
    }

    public static CheckersGame FromDto(this CheckersGameDto dto)
    {
        return new CheckersGame
        {
            Id = dto.Id,
            TournamentId = dto.TournamentId,
            WhitePlayerId = dto.WhitePlayerId,
            BlackPlayerId = dto.BlackPlayerId,
            WhiteTeamId = dto.WhiteTeamId,
            BlackTeamId = dto.BlackTeamId,
            WhiteTeamScore = dto.WhiteTeamScore,
            BlackTeamScore = dto.BlackTeamScore,
            Result = dto.Result,
            Round = dto.Round,
            ScheduledAt = dto.ScheduledAt,
            FinishedAt = dto.FinishedAt
        };
    }
}
