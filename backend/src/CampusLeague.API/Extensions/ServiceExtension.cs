using CampusLeague.Application.JWT;
using CampusLeague.Application.Security;
using CampusLeague.Application.Services.Implementations;
using CampusLeague.Application.Services.Implementations.Chess;
using CampusLeague.Application.Services.Interfaces;
using CampusLeague.Application.Services.Interfaces.Chess;

namespace CampusLeague.API.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordEncoder, BcryptPasswordEncoder>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IChessEventService, ChessEventService>();
        services.AddScoped<IChessPlayerService, ChessPlayerService>();
        services.AddScoped<IChessGameService, ChessGameService>();

        return services;
    }
}
