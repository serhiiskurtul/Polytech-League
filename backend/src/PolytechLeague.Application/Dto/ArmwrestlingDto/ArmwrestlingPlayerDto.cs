using System.Text.Json.Serialization;
using PolytechLeague.Domain.Entities.Armwrestling;
using PolytechLeague.Domain.Entities.Armwrestling.Enums;

namespace PolytechLeague.Application.Dto.ArmwrestlingDto;

public class ArmwrestlingPlayerRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? PhotoUrl { get; set; }
    public string? City { get; set; }
    public string? Group { get; set; }
    public float CurrentWeight { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ArmwrestlingWeightClass PreferredWeightClass { get; set; }
}

public class ArmwrestlingPlayerResponse
{
    public long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? PhotoUrl { get; set; }
    public string? City { get; set; }
    public string? Group { get; set; }
    public float Rating { get; set; }
    public float CurrentWeight { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ArmwrestlingWeightClass PreferredWeightClass { get; set; }
}

public static class ArmwrestlingPlayerDtoExtensions
{
    public static ArmwrestlingPlayer FromRequest(this ArmwrestlingPlayerRequest request)
    {
        return new ArmwrestlingPlayer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            PhotoUrl = request.PhotoUrl,
            City = request.City,
            Group = request.Group,
            CurrentWeight = request.CurrentWeight,
            PreferredWeightClass = request.PreferredWeightClass
        };
    }

    public static ArmwrestlingPlayerResponse ToResponse(this ArmwrestlingPlayer entity)
    {
        return new ArmwrestlingPlayerResponse
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            PhotoUrl = entity.PhotoUrl,
            City = entity.City,
            Group = entity.Group,
            CurrentWeight = entity.CurrentWeight,
            PreferredWeightClass = entity.PreferredWeightClass
        };
    }
}
