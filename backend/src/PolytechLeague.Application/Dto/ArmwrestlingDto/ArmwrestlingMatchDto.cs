using PolytechLeague.Domain.Entities;
using PolytechLeague.Application.Dto.GeneralDto;
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
    public PlayerDto? Player1 { get; set; }
    public long Player2Id { get; set; }
    public PlayerDto? Player2 { get; set; }
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

    private static PlayerDto? ToPlayerDto(Player? player, long? playerId, IEnumerable<ArmwrestlingEventParticipant>? participants)
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

    public static ArmwrestlingMatchResponse ToResponse(this ArmwrestlingMatch entity, IEnumerable<ArmwrestlingEventParticipant>? participants = null)
    {
        return new ArmwrestlingMatchResponse
        {
            Id = entity.Id,
            EventId = entity.EventId,
            Player1Id = entity.Player1Id,
            Player1 = ToPlayerDto(entity.Player1, entity.Player1Id, participants),
            Player2Id = entity.Player2Id,
            Player2 = ToPlayerDto(entity.Player2, entity.Player2Id, participants),
            WinnerId = entity.WinnerId,
            Stage = entity.Stage
        };
    }
}
