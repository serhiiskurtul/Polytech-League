using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Domain.Entities.Shared;

namespace PolytechLeague.Application.Dto.CheckersDto;

public class CheckersGameDto
{
    public long Id { get; set; }
    public long CheckersEventId { get; set; }

    public long WhitePlayerId { get; set; }
    public long BlackPlayerId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GameResult Result { get; set; }

    public int RoundNumber { get; set; }

    public DateTime ScheduledAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}


public static class CheckersGameDtoExtensions
{
    public static CheckersGameDto ToDto(this CheckersGame game)
    {
        return new CheckersGameDto
        {
            Id = game.Id,
            CheckersEventId = game.CheckersEventId,
            WhitePlayerId = game.WhitePlayerId,
            BlackPlayerId = game.BlackPlayerId,
            Result = game.Result,
            RoundNumber = game.RoundNumber,
            ScheduledAt = game.ScheduledAt,
            FinishedAt = game.FinishedAt
        };
    }

    public static CheckersGame FromDto(this CheckersGameDto dto)
    {
        return new CheckersGame
        {
            Id = dto.Id,
            CheckersEventId = dto.CheckersEventId,
            WhitePlayerId = dto.WhitePlayerId,
            BlackPlayerId = dto.BlackPlayerId,
            Result = dto.Result,
            RoundNumber = dto.RoundNumber,
            ScheduledAt = dto.ScheduledAt,
            FinishedAt = dto.FinishedAt
        };
    }
}
