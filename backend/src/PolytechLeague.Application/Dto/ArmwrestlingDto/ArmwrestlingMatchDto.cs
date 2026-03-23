using PolytechLeague.Domain.Entities.Armwrestling;

namespace PolytechLeague.Application.Dto.ArmwrestlingDto;

public class ArmwrestlingMatchRequest
{
    public long EventId { get; set; }
    public long Player1Id { get; set; }
    public long Player2Id { get; set; }
    public long? WinnerId { get; set; }
    public string? Stage { get; set; }
    public int TableNumber { get; set; }
}

public class ArmwrestlingMatchResponse
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public long Player1Id { get; set; }
    public ArmwrestlingPlayerResponse? Player1 { get; set; }
    public long Player2Id { get; set; }
    public ArmwrestlingPlayerResponse? Player2 { get; set; }
    public long? WinnerId { get; set; }
    public string? Stage { get; set; }
    public int TableNumber { get; set; }
}

public static class ArmwrestlingMatchDtoExtensions
{
    public static ArmwrestlingMatch FromRequest(this ArmwrestlingMatchRequest request)
    {
        return new ArmwrestlingMatch
        {
            EventId = request.EventId,
            Player1Id = request.Player1Id,
            Player2Id = request.Player2Id,
            WinnerId = request.WinnerId,
            Stage = request.Stage
        };
    }

    public static ArmwrestlingMatchResponse ToResponse(this ArmwrestlingMatch entity)
    {
        return new ArmwrestlingMatchResponse
        {
            Id = entity.Id,
            EventId = entity.EventId,
            Player1Id = entity.Player1Id,
            Player1 = entity.Player1?.ToResponse(),
            Player2Id = entity.Player2Id,
            Player2 = entity.Player2?.ToResponse(),
            WinnerId = entity.WinnerId,
            Stage = entity.Stage
        };
    }
}
