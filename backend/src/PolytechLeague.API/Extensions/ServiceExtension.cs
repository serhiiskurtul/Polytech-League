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
using PolytechLeague.Application.Services.Implementations.Futsal;
using PolytechLeague.Application.Services.Interfaces.Futsal;
using PolytechLeague.Application.Services.Implementations.Basketball3x3;
using PolytechLeague.Application.Services.Interfaces.Basketball3x3;
using PolytechLeague.Application.Services.Implementations.Volleyball;
using PolytechLeague.Application.Services.Interfaces.Volleyball;
using PolytechLeague.Application.Services.Interfaces.BeachVolleyball;
using PolytechLeague.Application.Services.Implementations.Badminton;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Application.Services.Implementations.BeachVolleyball;
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
        services.AddScoped<IFutsalPlayerService, FutsalPlayerService>();
        services.AddScoped<IFutsalTeamService, FutsalTeamService>();
        services.AddScoped<IFutsalEventService, FutsalEventService>();
        services.AddScoped<IFutsalMatchService, FutsalMatchService>();
        services.AddScoped<IEmailVerificationService, EmailVerificationService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddSingleton<IRedisService, RedisService>();
        services.AddScoped<IBasketball3x3PlayerService, Basketball3x3PlayerService>();
        services.AddScoped<IBasketball3x3TeamService, Basketball3x3TeamService>();
        services.AddScoped<IBasketball3x3EventService, Basketball3x3EventService>();
        services.AddScoped<IBasketball3x3MatchService, Basketball3x3MatchService>();
        services.AddScoped<IVolleyballPlayerService, VolleyballPlayerService>();
        services.AddScoped<IVolleyballTeamService, VolleyballTeamService>();
        services.AddScoped<IVolleyballEventService, VolleyballEventService>();
        services.AddScoped<IVolleyballMatchService, VolleyballMatchService>();
        services.AddScoped<IVolleyballSetService, VolleyballSetService>();
        services.AddScoped<IBeachVolleyballPlayerService, BeachVolleyballPlayerService>();
        services.AddScoped<IBeachVolleyballTeamService, BeachVolleyballTeamService>();
        services.AddScoped<IBeachVolleyballEventService, BeachVolleyballEventService>();
        services.AddScoped<IBeachVolleyballMatchService, BeachVolleyballMatchService>();
        services.AddScoped<IBeachVolleyballSetService, BeachVolleyballSetService>();

        // Badminton
        services.AddScoped<IBadmintonPlayerService, BadmintonPlayerService>();
        services.AddScoped<IBadmintonEventService, BadmintonEventService>();
        services.AddScoped<IBadmintonMatchService, BadmintonMatchService>();
        services.AddScoped<IBadmintonParticipantService, BadmintonParticipantService>();
        services.AddScoped<ITournamentService, TournamentService>();

        return services;
    }
}
