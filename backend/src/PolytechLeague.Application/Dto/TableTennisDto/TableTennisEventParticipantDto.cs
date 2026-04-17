using PolytechLeague.Domain.Entities.TableTennis;

namespace PolytechLeague.Application.Dto.TableTennisDto;

public class TableTennisEventParticipantDto
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public long TournamentId { get; set; }
    public long PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public double Points { get; set; }
    public int Rank { get; set; }
}

public static class TableTennisEventParticipantDtoExtensions
{
    public static TableTennisEventParticipantDto ToDto(this TableTennisEventParticipant participant)
    {
        return new TableTennisEventParticipantDto
        {
            Id = participant.PlayerId,
            UserId = participant.Player?.UserId,
            TournamentId = participant.TournamentId,
            PlayerId = participant.PlayerId,
            PlayerName = participant.Player != null ? $"{participant.Player.FirstName} {participant.Player.Patronymic ?? ""} {participant.Player.LastName}".Replace("  ", " ").Trim() : null,
            Points = participant.Points,
            Rank = participant.Rank
        };
    }

    public static TableTennisEventParticipant FromDto(this TableTennisEventParticipantDto dto, long tournamentId)
    {
        return new TableTennisEventParticipant
        {
            TournamentId = tournamentId,
            PlayerId = dto.PlayerId,
            Points = dto.Points,
            Rank = dto.Rank
        };
    }
}
