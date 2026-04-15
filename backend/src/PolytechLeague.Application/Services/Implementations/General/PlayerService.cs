using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Dto.GeneralDto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations.General;

public class PlayerService : IPlayerService
{
    private readonly ApplicationDbContext _dbContext;

    public PlayerService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PlayerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var players = await _dbContext.Players
            .ToListAsync(cancellationToken);
        return players.Select(p => p.ToDto());
    }

    public async Task<PlayerDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var player = await _dbContext.Players
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        return player?.ToDto();
    }

    public async Task<PlayerDto?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        var player = await _dbContext.Players
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        return player?.ToDto();
    }

    public async Task<PlayerDto> CreateAsync(PlayerDto dto, CancellationToken cancellationToken = default)
    {
        var player = dto.FromDto();
        await _dbContext.Players.AddAsync(player, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return player.ToDto();
    }

    public async Task UpdateAsync(long id, PlayerDto dto, CancellationToken cancellationToken = default)
    {
        var player = await _dbContext.Players.FindAsync(new object[] { id }, cancellationToken);
        if (player == null) return;

        // Update basic info
        player.FirstName = dto.FirstName ?? player.FirstName;
        player.LastName = dto.LastName ?? player.LastName;
        player.Group = dto.Group ?? player.Group;
        player.City = dto.City ?? player.City;
        player.UserId = dto.UserId ?? player.UserId;

        // Update sport stats (only if provided in DTO)
        if (dto.ChessRating.HasValue) player.ChessRating = dto.ChessRating;
        if (dto.ChessFideId != null) player.ChessFideId = dto.ChessFideId;
        if (dto.ChessTitle != null) player.ChessTitle = dto.ChessTitle;
        if (dto.CheckersRating.HasValue) player.CheckersRating = dto.CheckersRating;
        if (dto.TableTennisRating.HasValue) player.TableTennisRating = dto.TableTennisRating;
        if (dto.TableTennisHand.HasValue) player.TableTennisHand = dto.TableTennisHand;
        if (dto.ArmwrestlingWeight.HasValue) player.ArmwrestlingWeight = dto.ArmwrestlingWeight;
        if (dto.ArmwrestlingPreferredWeightClass.HasValue) player.ArmwrestlingPreferredWeightClass = dto.ArmwrestlingPreferredWeightClass;
        
        // Update team stats
        if (dto.FutsalPosition.HasValue) player.FutsalPosition = dto.FutsalPosition;
        if (dto.FutsalShirtNumber.HasValue) player.FutsalShirtNumber = dto.FutsalShirtNumber;
        if (dto.FutsalTeamId.HasValue) player.FutsalTeamId = dto.FutsalTeamId;
        
        if (dto.VolleyballPosition.HasValue) player.VolleyballPosition = dto.VolleyballPosition;
        if (dto.VolleyballShirtNumber.HasValue) player.VolleyballShirtNumber = dto.VolleyballShirtNumber;
        if (dto.VolleyballTeamId.HasValue) player.VolleyballTeamId = dto.VolleyballTeamId;

        if (dto.BasketballShirtNumber.HasValue) player.BasketballShirtNumber = dto.BasketballShirtNumber;
        if (dto.BasketballTeamId.HasValue) player.BasketballTeamId = dto.BasketballTeamId;

        if (dto.BeachVolleyballShirtNumber.HasValue) player.BeachVolleyballShirtNumber = dto.BeachVolleyballShirtNumber;
        if (dto.BeachVolleyballTeamId.HasValue) player.BeachVolleyballTeamId = dto.BeachVolleyballTeamId;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var player = await _dbContext.Players.FindAsync(new object[] { id }, cancellationToken);
        if (player != null)
        {
            _dbContext.Players.Remove(player);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<PlayerDto>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var lowerQuery = query.ToLower();
        var players = await _dbContext.Players
            .Where(p => p.FirstName.ToLower().Contains(lowerQuery) || 
                        p.LastName.ToLower().Contains(lowerQuery) ||
                        p.Group.ToLower().Contains(lowerQuery))
            .ToListAsync(cancellationToken);
        
        return players.Select(p => p.ToDto());
    }
}
