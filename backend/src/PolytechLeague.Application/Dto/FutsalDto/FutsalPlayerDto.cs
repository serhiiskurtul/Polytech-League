using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities.Futsal;
using PolytechLeague.Domain.Entities.Futsal.Enums;

namespace PolytechLeague.Application.Dto.FutsalDto;

public class FutsalPlayerRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public required string PhotoUrl { get; set; }
    public required string City { get; set; }
    public required string Group { get; set; }
    public long? TeamId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FutsalPosition Position { get; set; }
    public int? ShirtNumber { get; set; }
}

public class FutsalPlayerResponse
{
    public long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public required string PhotoUrl { get; set; }
    public required string City { get; set; }
    public required string Group { get; set; }
    public long? TeamId { get; set; }
    public FutsalTeamResponse? Team { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FutsalPosition Position { get; set; }
    public int? ShirtNumber { get; set; }
}

public static class FutsalPlayerDtoExtensions
{
    public static FutsalPlayer FromRequest(this FutsalPlayerRequest request)
    {
        return new FutsalPlayer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            PhotoUrl = request.PhotoUrl,
            City = request.City,
            Group = request.Group,
            TeamId = request.TeamId,
            Position = request.Position,
            ShirtNumber = request.ShirtNumber
        };
    }

    public static FutsalPlayerResponse ToResponse(this FutsalPlayer entity)
    {
        return new FutsalPlayerResponse
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            PhotoUrl = entity.PhotoUrl,
            City = entity.City,
            Group = entity.Group,
            TeamId = entity.TeamId,
            Team = entity.Team?.ToResponse(),
            Position = entity.Position,
            ShirtNumber = entity.ShirtNumber
        };
    }
}
