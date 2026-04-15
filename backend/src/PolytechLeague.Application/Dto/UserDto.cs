using PolytechLeague.Domain.Entities;
using System.Text.Json.Serialization;

namespace PolytechLeague.Application.Dto;

public class UserRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
    public required string Password { get; set; }
}

public class UserResponse
{
    public required long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public UserRole Role { get; set; }
    public string? Email { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

public class UserWithDeletedResponse
{
    public required long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public UserRole Role { get; set; }
    public string? Email { get; set; }
    public bool IsDeleted { get; set; }
}

public static class UserDtoExtension
{
    public static User FromRequest(this UserRequest userRequest)
    {
        return new User
        {
            FirstName = userRequest.FirstName,
            LastName = userRequest.LastName,
            Email = string.IsNullOrWhiteSpace(userRequest.Email) ? null : userRequest.Email,
            CreatedAt = DateTime.UtcNow,
            Password = userRequest.Password
        };
    }

    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    public static UserWithDeletedResponse ToResponseWithDeleted(this User user)
    {
        return new UserWithDeletedResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            Email = user.Email,
            IsDeleted = user.IsDeleted
        };
    }
}
