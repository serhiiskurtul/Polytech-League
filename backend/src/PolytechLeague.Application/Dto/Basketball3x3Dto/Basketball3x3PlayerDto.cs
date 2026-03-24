using PolytechLeague.Domain.Entities.Basketball3x3;

namespace PolytechLeague.Application.Dto.Basketball3x3Dto;

public class Basketball3x3PlayerRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public required string PhotoUrl { get; set; }
    public required string City { get; set; }
    public required string Group { get; set; }
    public long? TeamId { get; set; }

    public int? ShirtNumber { get; set; }
}

public class Basketball3x3PlayerResponse
{
    public long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public required string PhotoUrl { get; set; }
    public required string City { get; set; }
    public required string Group { get; set; }
    public long? TeamId { get; set; }
    public Basketball3x3TeamResponse? Team { get; set; }

    public int? ShirtNumber { get; set; }
}

public static class Basketball3x3PlayerDtoExtensions
{
    public static Basketball3x3Player FromRequest(this Basketball3x3PlayerRequest request)
    {
        return new Basketball3x3Player
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            PhotoUrl = request.PhotoUrl,
            City = request.City,
            Group = request.Group,
            TeamId = request.TeamId,
            ShirtNumber = request.ShirtNumber
        };
    }

    public static Basketball3x3PlayerResponse ToResponse(this Basketball3x3Player entity)
    {
        return new Basketball3x3PlayerResponse
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
            ShirtNumber = entity.ShirtNumber
        };
    }
}
