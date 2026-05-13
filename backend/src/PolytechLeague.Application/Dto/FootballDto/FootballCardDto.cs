using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Domain.Entities.Football;

namespace PolytechLeague.Application.Dto.FootballDto;

public class FootballCardRequest
{
    public long PlayerId { get; set; }
}

public class FootballCardResponse
{
    public long Id { get; set; }
    public long MatchId { get; set; }
    public long PlayerId { get; set; }
    public PlayerDto? Player { get; set; }
    public FootballCardType Type { get; set; }
}

public static class FootballCardDtoExtensions
{
    public static FootballCard FromRequest(this FootballCardRequest request, long matchId, FootballCardType type)
    {
        return new FootballCard
        {
            MatchId = matchId,
            PlayerId = request.PlayerId,
            Type = type
        };
    }

    public static FootballCardResponse ToResponse(this FootballCard entity)
    {
        return new FootballCardResponse
        {
            Id = entity.Id,
            MatchId = entity.MatchId,
            PlayerId = entity.PlayerId,
            Player = entity.Player?.ToDto(),
            Type = entity.Type
        };
    }
}
