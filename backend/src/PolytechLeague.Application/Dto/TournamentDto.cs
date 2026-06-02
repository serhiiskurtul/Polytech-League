using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Dto;

public class TournamentDto
{
    public long Id { get; set; }
    public required string Title { get; set; }
    public required string Sport { get; set; }
    public required string Status { get; set; }
    public required string Date { get; set; }
    public required string Location { get; set; }
    public required string Description { get; set; }
    public required string Level { get; set; }
    public required string Category { get; set; }
    public int Participants { get; set; }
    public int Teams { get; set; }
    public string? Prize { get; set; }
    public string? Organizer { get; set; }
    public string? RegistrationStart { get; set; }
    public string? RegistrationEnd { get; set; }
    public string? EndDate { get; set; }
    public string? CurrentRound { get; set; }
    public bool Registered { get; set; }
    public bool Featured { get; set; }
    public bool IsTeam { get; set; }
    public string? Time { get; set; }
    public bool IsModerated { get; set; }
    public bool IsPublic { get; set; }
    public bool DisableNotifications { get; set; }
    public long? SeasonId { get; set; }
    public string? MainPhotoUrl { get; set; }
    public DateTime RawDate { get; set; }
}
