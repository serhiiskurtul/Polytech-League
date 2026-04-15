using PolytechLeague.Application.Dto.GeneralDto;

namespace PolytechLeague.Application.Services.Interfaces;

public interface IPlayerService
{
    Task<IEnumerable<PlayerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PlayerDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<PlayerDto?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    Task<string?> GetPhotoUrlByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    Task<PlayerDto> CreateAsync(PlayerDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(long id, PlayerDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlayerDto>> SearchAsync(string query, CancellationToken cancellationToken = default);
}
