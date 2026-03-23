using PolytechLeague.Application.JWT;
using PolytechLeague.Application.Security;
using PolytechLeague.Application.Services.Implementations;
using PolytechLeague.Application.Services.Implementations.Chess;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Application.Services.Interfaces.Chess;
using PolytechLeague.Application.Services.Implementations.Checkers;
using PolytechLeague.Application.Services.Interfaces.Checkers;
using PolytechLeague.Application.Services.Implementations.TableTennis;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Application.Services.Implementations.Armwrestling;
using PolytechLeague.Application.Services.Interfaces.Armwrestling;
using PolytechLeague.Infrastructure.Email;
using PolytechLeague.Infrastructure.Redis;
using StackExchange.Redis;

namespace PolytechLeague.API.Extensions;

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
        services.AddScoped<ICheckersEventService, CheckersEventService>();
        services.AddScoped<ICheckersPlayerService, CheckersPlayerService>();
        services.AddScoped<ICheckersGameService, CheckersGameService>();
        services.AddScoped<ITableTennisEventService, TableTennisEventService>();
        services.AddScoped<ITableTennisPlayerService, TableTennisPlayerService>();
        services.AddScoped<ITableTennisMatchService, TableTennisMatchService>();
        services.AddScoped<ITableTennisSetService, TableTennisSetService>();
        services.AddScoped<IArmwrestlingPlayerService, ArmwrestlingPlayerService>();
        services.AddScoped<IArmwrestlingEventService, ArmwrestlingEventService>();
        services.AddScoped<IArmwrestlingMatchService, ArmwrestlingMatchService>();
        services.AddScoped<IEmailVerificationService, EmailVerificationService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddSingleton<IRedisService, RedisService>();

        return services;
    }
}
