using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities.Volleyball;
using PolytechLeague.Domain.Entities.Volleyball.Enums;

namespace PolytechLeague.Application.Dto.VolleyballDto;

public class VolleyballPlayerRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public required string PhotoUrl { get; set; }
    public required string City { get; set; }
    public required string Group { get; set; }
    public long? TeamId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public VolleyballPosition Position { get; set; }

    public int? ShirtNumber { get; set; }
}

public class VolleyballPlayerResponse
{
    public long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public required string PhotoUrl { get; set; }
    public required string City { get; set; }
    public required string Group { get; set; }
    public long? TeamId { get; set; }
    public VolleyballTeamResponse? Team { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public VolleyballPosition Position { get; set; }

    public int? ShirtNumber { get; set; }
}

public static class VolleyballPlayerDtoExtensions
{
    public static VolleyballPlayer FromRequest(this VolleyballPlayerRequest request)
    {
        return new VolleyballPlayer
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

    public static VolleyballPlayerResponse ToResponse(this VolleyballPlayer entity)
    {
        return new VolleyballPlayerResponse
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
