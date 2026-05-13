using PolytechLeague.Application.JWT;
using PolytechLeague.Application.Security;
using PolytechLeague.Application.Services.Implementations.General;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Application.Services;
using PolytechLeague.Application.Services.Implementations;
using PolytechLeague.Application.Services.Implementations.Chess;
using PolytechLeague.Application.Services.Interfaces.Chess;
using PolytechLeague.Application.Services.Implementations.Checkers;
using PolytechLeague.Application.Services.Interfaces.Checkers;
using PolytechLeague.Application.Services.Implementations.TableTennis;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Application.Services.Implementations.Armwrestling;
using PolytechLeague.Application.Services.Interfaces.Armwrestling;
using PolytechLeague.Application.Services.Implementations.Football;
using PolytechLeague.Application.Services.Interfaces.Football;
using PolytechLeague.Application.Services.Implementations.Basketball3x3;
using PolytechLeague.Application.Services.Interfaces.Basketball3x3;
using PolytechLeague.Application.Services.Implementations.Volleyball;
using PolytechLeague.Application.Services.Interfaces.Volleyball;
using PolytechLeague.Application.Services.Interfaces.BeachVolleyball;
using PolytechLeague.Application.Services.Implementations.Badminton;
using PolytechLeague.Application.Services.Interfaces.Badminton;
using PolytechLeague.Application.Services.Implementations.BeachVolleyball;
using PolytechLeague.Application.Services.Implementations.Athletics;
using PolytechLeague.Application.Services.Interfaces.Athletics;
using PolytechLeague.Infrastructure.Email;
using PolytechLeague.Infrastructure.Redis;
using StackExchange.Redis;
using PolytechLeague.Application.Configuration;
using PolytechLeague.Infrastructure.Notifications;

namespace PolytechLeague.API.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPasswordEncoder, BcryptPasswordEncoder>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IPlayerService, PlayerService>(); // Unified Player Service
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IExcelImportService, ExcelImportService>();


        services.AddScoped<IChessEventService, ChessEventService>();
        services.AddScoped<IChessGameService, ChessGameService>();
        services.AddScoped<IChessTeamService, ChessTeamService>();
        services.AddScoped<ICheckersEventService, CheckersEventService>();
        services.AddScoped<ICheckersGameService, CheckersGameService>();
        services.AddScoped<ICheckersTeamService, CheckersTeamService>();
        services.AddScoped<ITableTennisEventService, TableTennisEventService>();
        services.AddScoped<ITableTennisMatchService, TableTennisMatchService>();
        services.AddScoped<ITableTennisSetService, TableTennisSetService>();
        services.AddScoped<ITableTennisTeamService, TableTennisTeamService>();

        services.AddScoped<IArmwrestlingEventService, ArmwrestlingEventService>();
        services.AddScoped<IArmwrestlingMatchService, ArmwrestlingMatchService>();
        services.AddScoped<IFootballTeamService, FootballTeamService>();
        services.AddScoped<IFootballEventService, FootballEventService>();
        services.AddScoped<IFootballMatchService, FootballMatchService>();
        services.AddScoped<IEmailVerificationService, EmailVerificationService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddSingleton<IRedisService, RedisService>();
        services.AddScoped<IBasketball3x3TeamService, Basketball3x3TeamService>();
        services.AddScoped<IBasketball3x3EventService, Basketball3x3EventService>();
        services.AddScoped<IBasketball3x3MatchService, Basketball3x3MatchService>();
        services.AddScoped<IVolleyballTeamService, VolleyballTeamService>();
        services.AddScoped<IVolleyballEventService, VolleyballEventService>();
        services.AddScoped<IVolleyballMatchService, VolleyballMatchService>();
        services.AddScoped<IVolleyballSetService, VolleyballSetService>();
        services.AddScoped<IBeachVolleyballTeamService, BeachVolleyballTeamService>();
        services.AddScoped<IBeachVolleyballEventService, BeachVolleyballEventService>();
        services.AddScoped<IBeachVolleyballMatchService, BeachVolleyballMatchService>();
        services.AddScoped<IBeachVolleyballSetService, BeachVolleyballSetService>();

        // Badminton
        services.AddScoped<IBadmintonEventService, BadmintonEventService>();
        services.AddScoped<IBadmintonMatchService, BadmintonMatchService>();
        services.AddScoped<IBadmintonParticipantService, BadmintonParticipantService>();
        services.AddScoped<IBadmintonTeamService, BadmintonTeamService>();
        services.AddScoped<IAthleticsEventService, AthleticsEventService>();
        services.AddScoped<ITournamentService, TournamentService>();
        
        services.Configure<CloudflareR2Settings>(configuration.GetSection("CloudflareR2"));
        services.AddScoped<IFileService, CloudflareR2FileService>();

        services.AddScoped<ISeasonRatingService, SeasonRatingService>();
        services.AddScoped<ICollegeService, CollegeService>();

        services.Configure<VapidSettings>(configuration.GetSection("VapidSettings"));
        services.AddScoped<IPushNotificationService, WebPushNotificationService>();

        return services;
    }
}
