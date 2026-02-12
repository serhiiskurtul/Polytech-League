using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.JWT;

public interface IJwtService
{
    string GenerateAccessToken(User user);

    RefreshToken GenerateRefreshToken(long userId, int expireDays = 7);
}
