using CampusLeague.Domain.Entities;

namespace CampusLeague.Application.JWT;

public interface IJwtService
{
    string GenerateAccessToken(User user);

    RefreshToken GenerateRefreshToken(long userId, int expireDays = 7);
}
