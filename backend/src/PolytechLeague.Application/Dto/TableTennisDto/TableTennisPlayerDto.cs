using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities.TableTennis;
using PolytechLeague.Domain.Entities.TableTennis.Enums;

namespace PolytechLeague.Application.Dto.TableTennisDto;

public class TableTennisPlayerRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateOnly BirthDate { get; set; }
    public string? PhotoUrl { get; set; }
    public required string City { get; set; }
    public required string Group { get; set; }
    public int Rating { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TableTennisHand PlayingHand { get; set; }
}

public class TableTennisPlayerResponse
{
    public long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateOnly BirthDate { get; set; }
    public string? PhotoUrl { get; set; }
    public required string City { get; set; }
    public required string Group { get; set; }
    public int Rating { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TableTennisHand PlayingHand { get; set; }
}

public static class TableTennisPlayerDtoExtensions
{
    public static TableTennisPlayer FromRequest(this TableTennisPlayerRequest request)
    {
        return new TableTennisPlayer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            PhotoUrl = request.PhotoUrl,
            City = request.City,
            Group = request.Group,
            Rating = request.Rating,
            PlayingHand = request.PlayingHand
        };
    }

    public static TableTennisPlayerResponse ToResponse(this TableTennisPlayer entity)
    {
        return new TableTennisPlayerResponse
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            PhotoUrl = entity.PhotoUrl,
            City = entity.City,
            Group = entity.Group,
            Rating = entity.Rating,
            PlayingHand = entity.PlayingHand
        };
    }
}
