using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities.Chess;
using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Application.Dto.ChessDto;

public class ChessGameDto
{
    public long Id { get; set; }
    public long TournamentId { get; set; }

    public long? WhitePlayerId { get; set; }
    public long? WhiteUserId { get; set; }
    public string? WhitePlayerName { get; set; }
    public long? BlackPlayerId { get; set; }
    public long? BlackUserId { get; set; }
    public string? BlackPlayerName { get; set; }

    public long? WhiteTeamId { get; set; }
    public ChessTeamResponse? WhiteTeam { get; set; }
    public long? BlackTeamId { get; set; }
    public ChessTeamResponse? BlackTeam { get; set; }
    public double? WhiteTeamScore { get; set; }
    public double? BlackTeamScore { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GameResult Result { get; set; }

    public int Round { get; set; }
    public int Board { get; set; }
    public string? Pgn { get; set; }
}


public static class ChessGameDtoExtensions
{
    public static ChessGameDto ToDto(this ChessGame game, IEnumerable<ChessEventParticipant>? participants = null)
    {
        string? whiteName = null;
        string? blackName = null;

        if (participants != null)
        {
            var whiteP = participants.FirstOrDefault(p => p.PlayerId == game.WhitePlayerId);
            if (whiteP != null)
            {
                whiteName = (whiteP.PlayerFirstName != null || whiteP.PlayerLastName != null)
                    ? $"{whiteP.PlayerFirstName} {whiteP.PlayerLastName}".Trim()
                    : (game.WhitePlayer != null ? $"{game.WhitePlayer.FirstName} {game.WhitePlayer.LastName}".Trim() : null);
            }
            var blackP = participants.FirstOrDefault(p => p.PlayerId == game.BlackPlayerId);
            if (blackP != null)
            {
                blackName = (blackP.PlayerFirstName != null || blackP.PlayerLastName != null)
                    ? $"{blackP.PlayerFirstName} {blackP.PlayerLastName}".Trim()
                    : (game.BlackPlayer != null ? $"{game.BlackPlayer.FirstName} {game.BlackPlayer.LastName}".Trim() : null);
            }
        }

        if (whiteName == null)
        {
            whiteName = game.WhitePlayer != null ? $"{game.WhitePlayer.FirstName} {game.WhitePlayer.LastName}".Trim() : null;
        }
        if (blackName == null)
        {
            blackName = game.BlackPlayer != null ? $"{game.BlackPlayer.FirstName} {game.BlackPlayer.LastName}".Trim() : null;
        }

        return new ChessGameDto
        {
            Id = game.Id,
            TournamentId = game.TournamentId,
            WhitePlayerId = game.WhitePlayerId,
            WhiteUserId = game.WhitePlayer?.UserId,
            WhitePlayerName = whiteName,
            BlackPlayerId = game.BlackPlayerId,
            BlackUserId = game.BlackPlayer?.UserId,
            BlackPlayerName = blackName,
            WhiteTeamId = game.WhiteTeamId,
            WhiteTeam = game.WhiteTeam?.ToResponse(),
            BlackTeamId = game.BlackTeamId,
            BlackTeam = game.BlackTeam?.ToResponse(),
            WhiteTeamScore = game.WhiteTeamScore,
            BlackTeamScore = game.BlackTeamScore,
            Result = game.Result,
            Round = game.Round,
            Board = game.Board,
            Pgn = game.Pgn
        };
    }

    public static ChessGame FromDto(this ChessGameDto dto)
    {
        return new ChessGame
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
            Board = dto.Board,
            Pgn = dto.Pgn
        };
    }
}