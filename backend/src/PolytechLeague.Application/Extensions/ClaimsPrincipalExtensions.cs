using System.Security.Claims;
using System.Security.Authentication;

namespace PolytechLeague.Application.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal user)
    {
        // Шукаємо спочатку за довгим типом, потім за коротким "sub"
        var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                 ?? user.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(id))
        {
            // Виводимо всі наявні клейми в консоль для дебагу, якщо ID не знайдено
            var allClaims = string.Join(", ", user.Claims.Select(c => $"{c.Type}:{c.Value}"));
            throw new AuthenticationException($"User id claim not found. Available claims: {allClaims}");
        }

        return long.Parse(id);
    }
}