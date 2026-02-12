using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities.Chess;

namespace PolytechLeague.Application.Dto.ChessDto;

public class ChessGameDto
{
    public long Id { get; set; }
    public long TournamentId { get; set; }

    public long WhitePlayerId { get; set; }
    public long BlackPlayerId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChessGameResult Result { get; set; }

    public int Round { get; set; }
    public int Board { get; set; }
}


public static class ChessGameDtoExtensions
{
    public static ChessGameDto ToDto(this ChessGame game)
    {
        return new ChessGameDto
        {
            Id = game.Id,
            TournamentId = game.TournamentId,
            WhitePlayerId = game.WhitePlayerId,
            BlackPlayerId = game.BlackPlayerId,
            Result = game.Result,
            Round = game.Round,
            Board = game.Board
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
            Result = dto.Result,
            Round = dto.Round,
            Board = dto.Board
        };
    }
}