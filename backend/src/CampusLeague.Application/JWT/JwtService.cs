using CampusLeague.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CampusLeague.Application.JWT;

public class JwtService: IJwtService
{
    // присвоюємо Value, щоб не було потреби у подальшому звертатися options.Value.SecretKey
    private readonly JwtOptions _options;
    public JwtService(IOptions<JwtOptions> options)
    {
        if (options == null || options.Value == null) throw new ArgumentNullException(nameof(options), "JWT options are not configured.");

        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.SecretKey)) throw new ArgumentException("JWT SecretKey is null or empty.", nameof(_options.SecretKey));
        if (string.IsNullOrWhiteSpace(_options.Issuer)) throw new ArgumentException("JWT Issuer is null or empty.", nameof(_options.Issuer));
        if (string.IsNullOrWhiteSpace(_options.Audience)) throw new ArgumentException("JWT Audience is null or empty.", nameof(_options.Audience));
        if (_options.ExpiresHours <= 0) throw new ArgumentOutOfRangeException(nameof(_options.ExpiresHours), "JWT ExpiresHours must be greater than zero.");
    }

    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_options.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("name", $"{user.FirstName} {user.LastName}".Trim())
            }),
            Expires = DateTime.UtcNow.AddHours(_options.ExpiresHours),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public RefreshToken GenerateRefreshToken(long userId, int expireDays = 7)
    {
        var token = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddDays(expireDays),
            UserId = userId
        };

        return token;
    }
}
