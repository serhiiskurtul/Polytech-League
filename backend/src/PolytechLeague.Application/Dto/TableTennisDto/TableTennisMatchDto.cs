using PolytechLeague.Domain.Entities.TableTennis;

namespace PolytechLeague.Application.Dto.TableTennisDto;

public class TableTennisSetDto
{
    public long Id { get; set; }
    public int SetNumber { get; set; }
    public int Player1Score { get; set; }
    public int Player2Score { get; set; }
}

public class TableTennisMatchRequest
{
    public long TournamentId { get; set; }
    public long Player1Id { get; set; }
    public long Player2Id { get; set; }
    public long? WinnerId { get; set; }
    public string? Stage { get; set; }
    public int TableNumber { get; set; }
    public int SetsToWin { get; set; }
}

public class TableTennisMatchResponse
{
    public long Id { get; set; }
    public long TournamentId { get; set; }
    public long Player1Id { get; set; }
    public long Player2Id { get; set; }
    public long? WinnerId { get; set; }
    public string? Stage { get; set; }
    public int TableNumber { get; set; }
    public int SetsToWin { get; set; }
    public ICollection<TableTennisSetDto>? Sets { get; set; }
}

public static class TableTennisMatchDtoExtensions
{
    public static TableTennisMatch FromRequest(this TableTennisMatchRequest request)
    {
        return new TableTennisMatch
        {
            TournamentId = request.TournamentId,
            Player1Id = request.Player1Id,
            Player2Id = request.Player2Id,
            WinnerId = request.WinnerId,
            Stage = request.Stage,
            TableNumber = request.TableNumber,
            SetsToWin = request.SetsToWin
        };
    }

    public static TableTennisMatchResponse ToResponse(this TableTennisMatch entity)
    {
        return new TableTennisMatchResponse
        {
            Id = entity.Id,
            TournamentId = entity.TournamentId,
            Player1Id = entity.Player1Id,
            Player2Id = entity.Player2Id,
            WinnerId = entity.WinnerId,
            Stage = entity.Stage,
            TableNumber = entity.TableNumber,
            SetsToWin = entity.SetsToWin,
            Sets = entity.Sets?.Select(s => s.ToDto()).ToList()
        };
    }

    public static TableTennisSetDto ToDto(this TableTennisSet entity)
    {
        return new TableTennisSetDto
        {
            Id = entity.Id,
            SetNumber = entity.SetNumber,
            Player1Score = entity.Player1Score,
            Player2Score = entity.Player2Score
        };
    }
}
