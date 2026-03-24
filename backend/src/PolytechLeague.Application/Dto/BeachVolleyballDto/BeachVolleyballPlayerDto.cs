using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities.BeachVolleyball;
using PolytechLeague.Domain.Entities.BeachVolleyball.Enums;

namespace PolytechLeague.Application.Dto.BeachVolleyballDto;

public class BeachVolleyballPlayerRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public required string PhotoUrl { get; set; }
    public required string City { get; set; }
    public required string Group { get; set; }
    public long? TeamId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BeachVolleyballPosition Position { get; set; }

    public int? ShirtNumber { get; set; }
}

public class BeachVolleyballPlayerResponse
{
    public long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public required string PhotoUrl { get; set; }
    public required string City { get; set; }
    public required string Group { get; set; }
    public long? TeamId { get; set; }
    public BeachVolleyballTeamResponse? Team { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BeachVolleyballPosition Position { get; set; }

    public int? ShirtNumber { get; set; }
}

public static class BeachVolleyballPlayerDtoExtensions
{
    public static BeachVolleyballPlayer FromRequest(this BeachVolleyballPlayerRequest request)
    {
        return new BeachVolleyballPlayer
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

    public static BeachVolleyballPlayerResponse ToResponse(this BeachVolleyballPlayer entity)
    {
        return new BeachVolleyballPlayerResponse
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
