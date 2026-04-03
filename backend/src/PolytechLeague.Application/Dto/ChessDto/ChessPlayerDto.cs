using PolytechLeague.Domain.Entities.Chess;

namespace PolytechLeague.Application.Dto.ChessDto;

public class ChessPlayerDto
{
    public long? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Group { get; set; }
    public string? City { get; set; }
    public int? RatingELO { get; set; }
    public string? FideId { get; set; }
    public string? Title { get; set; }
}

public static class ChessPlayerDtoExtensions
{
    public static ChessPlayerDto ToDto(this ChessPlayer player)
    {
        return new ChessPlayerDto
        {
            Id = player.Id,
            FirstName = player.FirstName,
            LastName = player.LastName,
            Group = player.Group,
            City = player.City,
            RatingELO = player.RatingELO,
            FideId = player.FideId,
            Title = player.Title
        };
    }

    public static ChessPlayer FromDto(this ChessPlayerDto dto)
    {
        var player = new ChessPlayer()
        {
            Id = dto.Id ?? 0,
            FirstName = dto.FirstName ?? string.Empty,
            LastName = dto.LastName ?? string.Empty,
            Group = dto.Group ?? string.Empty,
            City = dto.City ?? "Київ",
            RatingELO = dto.RatingELO ?? 0,
            FideId = string.IsNullOrWhiteSpace(dto.FideId) ? null : dto.FideId.Trim(),
            Title = string.IsNullOrWhiteSpace(dto.Title) ? null : dto.Title.Trim(),
            BirthDate = DateOnly.MinValue,
            PhotoUrl = ""
        };
        return player;
    }
}
