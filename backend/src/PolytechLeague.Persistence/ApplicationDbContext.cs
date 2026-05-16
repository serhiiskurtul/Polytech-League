using PolytechLeague.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.PostgreSQL;
using PolytechLeague.Domain.Entities.Chess;
using PolytechLeague.Persistence.DbConfigurations;
using PolytechLeague.Persistence.DbConfigurations.ChessConfigurations;
using PolytechLeague.Domain.Entities.Shared;
using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Persistence.DbConfigurations.CheckersConfigurations;
using PolytechLeague.Domain.Entities.TableTennis;
using PolytechLeague.Domain.Entities.TableTennis.Enums;
using PolytechLeague.Domain.Entities.Armwrestling;
using PolytechLeague.Domain.Entities.Armwrestling.Enums;
using PolytechLeague.Persistence.DbConfigurations.TableTennisConfigurations;
using PolytechLeague.Persistence.DbConfigurations.ArmwrestlingConfigurations;
using PolytechLeague.Domain.Entities.Football;
using PolytechLeague.Domain.Entities.Football.Enums;
using PolytechLeague.Persistence.DbConfigurations.FootballConfigurations;
using PolytechLeague.Domain.Entities.Basketball;
using PolytechLeague.Domain.Entities.Basketball.Enums;
using PolytechLeague.Persistence.DbConfigurations.BasketballConfigurations;
using PolytechLeague.Domain.Entities.Volleyball;
using PolytechLeague.Domain.Entities.Volleyball.Enums;
using PolytechLeague.Persistence.DbConfigurations.VolleyballConfigurations;
using PolytechLeague.Domain.Entities.BeachVolleyball.Enums;
using PolytechLeague.Domain.Entities.Badminton;
using PolytechLeague.Domain.Entities.Badminton.Enums;
using PolytechLeague.Persistence.DbConfigurations.BadmintonConfigurations;
using PolytechLeague.Domain.Entities.BeachVolleyball;
using PolytechLeague.Persistence.DbConfigurations.BeachVolleyballConfigurations;
using PolytechLeague.Domain.Entities.Athletics;
using PolytechLeague.Persistence.DbConfigurations.AthleticsConfigurations;

namespace PolytechLeague.Persistence.Database;

public class ApplicationDbContext : DbContext   
{
    public ApplicationDbContext() { } //used for Service Tests
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventPhotos> EventPhotos { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<ProfileMergeRequest> ProfileMergeRequests { get; set; }
    public DbSet<College> Colleges { get; set; }
    public DbSet<NationalRatingManualEntry> NationalRatingManualEntries { get; set; }
    public DbSet<RegionalRatingManualEntry> RegionalRatingManualEntries { get; set; }
    public DbSet<HallOfFameManualEntry> HallOfFameManualEntries { get; set; }
    public DbSet<PushSubscription> PushSubscriptions { get; set; }
    public DbSet<ExternalAchievement> ExternalAchievements { get; set; }
    public DbSet<ExternalAchievementMedia> ExternalAchievementMedias { get; set; }


    
    // Unified Players
    public DbSet<Player> Players { get; set; }

    public DbSet<ChessEventParticipant> ChessEventParticipants { get; set; }
    public DbSet<ChessEvent> ChessEvents { get; set; }
    public DbSet<ChessGame> ChessGames { get; set; }
    public DbSet<ChessTeam> ChessTeams { get; set; }
    public DbSet<ChessReferee> ChessReferees { get; set; }
    public DbSet<CheckersGame> CheckersGames { get; set; }
    public DbSet<CheckersEventParticipant> CheckersEventParticipants { get; set; }
    public DbSet<CheckersReferee> CheckersReferees { get; set; }
    public DbSet<CheckersEvent> CheckersEvents { get; set; }
    public DbSet<CheckersTeam> CheckersTeams { get; set; }
    public DbSet<TableTennisEvent> TableTennisEvents { get; set; }
    public DbSet<TableTennisEventParticipant> TableTennisEventParticipants { get; set; }
    public DbSet<TableTennisTeam> TableTennisTeams { get; set; }
    public DbSet<TableTennisMatch> TableTennisMatches { get; set; }
    public DbSet<TableTennisSet> TableTennisSets { get; set; }


    // Armwrestling
    public DbSet<ArmwrestlingEvent> ArmwrestlingEvents { get; set; }
    public DbSet<ArmwrestlingEventParticipant> ArmwrestlingEventParticipants { get; set; }
    public DbSet<ArmwrestlingMatch> ArmwrestlingMatches { get; set; }

    // Football
    public DbSet<FootballTeam> FootballTeams { get; set; }
    public DbSet<FootballEvent> FootballEvents { get; set; }
    public DbSet<FootballEventParticipant> FootballEventParticipants { get; set; }
    public DbSet<FootballMatch> FootballMatches { get; set; }
    public DbSet<FootballGoal> FootballGoals { get; set; }
    public DbSet<FootballCard> FootballCards { get; set; }

    // Basketball 3x3
    public DbSet<BasketballTeam> BasketballTeams { get; set; }
    public DbSet<BasketballEvent> BasketballEvents { get; set; }
    public DbSet<BasketballEventParticipant> BasketballEventParticipants { get; set; }
    public DbSet<BasketballMatch> BasketballMatches { get; set; }

    // Volleyball
    public DbSet<VolleyballTeam> VolleyballTeams { get; set; }
    public DbSet<VolleyballEvent> VolleyballEvents { get; set; }
    public DbSet<VolleyballEventParticipant> VolleyballEventParticipants { get; set; }
    public DbSet<VolleyballMatch> VolleyballMatches { get; set; }
    public DbSet<VolleyballSet> VolleyballSets { get; set; }

    // Beach volleyball
    public DbSet<BeachVolleyballTeam> BeachVolleyballTeams { get; set; }
    public DbSet<BeachVolleyballEvent> BeachVolleyballEvents { get; set; }
    public DbSet<BeachVolleyballEventParticipant> BeachVolleyballEventParticipants { get; set; }
    public DbSet<BeachVolleyballMatch> BeachVolleyballMatches { get; set; }
    public DbSet<BeachVolleyballSet> BeachVolleyballSets { get; set; }

    // Badminton
    public DbSet<BadmintonTeam> BadmintonTeams { get; set; }
    public DbSet<BadmintonEvent> BadmintonEvents { get; set; }
    public DbSet<BadmintonEventParticipant> BadmintonEventParticipants { get; set; }
    public DbSet<BadmintonMatch> BadmintonMatches { get; set; }
    public DbSet<BadmintonSet> BadmintonSets { get; set; }

    // Athletics
    public DbSet<AthleticsEvent> AthleticsEvents { get; set; }
    public DbSet<AthleticsResult> AthleticsResults { get; set; }
    public DbSet<AthleticsEventParticipant> AthleticsEventParticipants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<UserRole>()
            .HasPostgresEnum<GameResult>()
            .HasPostgresEnum<CompetitionLevel>()
            .HasPostgresEnum<ChessSystem>()
            .HasPostgresEnum<CheckersSystem>()
            .HasPostgresEnum<CheckersVariant>()
            .HasPostgresEnum<TableTennisHand>()
            .HasPostgresEnum<TableTennisTournamentType>()
            .HasPostgresEnum<EventCategory>()
            .HasPostgresEnum<ArmwrestlingHand>()
            .HasPostgresEnum<ArmwrestlingWeightClass>()
            .HasPostgresEnum<FootballPosition>()
            .HasPostgresEnum<BasketballTournamentType>()
            .HasPostgresEnum<VolleyballTournamentType>()
            .HasPostgresEnum<VolleyballPosition>()
            .HasPostgresEnum<BeachVolleyballTournamentType>()
            .HasPostgresEnum<BadmintonTournamentType>()
            .HasPostgresEnum<NotificationType>()
            .HasPostgresEnum<RegistrationStatus>();

        modelBuilder.Entity<Person>().UseTptMappingStrategy();
        modelBuilder.Entity<Event>().UseTptMappingStrategy();

        //General configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new PlayerConfiguration()); // Unified Player config
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new EventPhotosConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new ConversationConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new SeasonConfiguration());


        // Chess configurations
        modelBuilder.ApplyConfiguration(new ChessRefereeConfiguration());
        modelBuilder.ApplyConfiguration(new ChessEventConfiguration());
        modelBuilder.ApplyConfiguration(new ChessEventParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new ChessGameConfiguration());
        modelBuilder.ApplyConfiguration(new ChessTeamConfiguration());

        // Checkers configurations
        modelBuilder.ApplyConfiguration(new CheckersRefereeConfiguration());
        modelBuilder.ApplyConfiguration(new CheckersEventConfiguration());
        modelBuilder.ApplyConfiguration(new CheckersEventParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new CheckersGameConfiguration());
        modelBuilder.ApplyConfiguration(new CheckersTeamConfiguration());

        // Table Tennis configurations
        modelBuilder.ApplyConfiguration(new TableTennisEventConfiguration());
        modelBuilder.ApplyConfiguration(new TableTennisEventParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new TableTennisTeamConfiguration());
        modelBuilder.ApplyConfiguration(new TableTennisMatchConfiguration());
        modelBuilder.ApplyConfiguration(new TableTennisSetConfiguration());


        // Armwrestling configurations
        modelBuilder.ApplyConfiguration(new ArmwrestlingEventConfiguration());
        modelBuilder.ApplyConfiguration(new ArmwrestlingEventParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new ArmwrestlingMatchConfiguration());

        // Football configurations
        modelBuilder.ApplyConfiguration(new FootballTeamConfiguration());
        modelBuilder.ApplyConfiguration(new FootballEventConfiguration());
        modelBuilder.ApplyConfiguration(new FootballEventParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new FootballMatchConfiguration());
        modelBuilder.ApplyConfiguration(new FootballGoalConfiguration());
        modelBuilder.ApplyConfiguration(new FootballCardConfiguration());

        // Basketball 3x3 configurations
        modelBuilder.ApplyConfiguration(new BasketballTeamConfiguration());
        modelBuilder.ApplyConfiguration(new BasketballEventConfiguration());
        modelBuilder.ApplyConfiguration(new BasketballEventParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new BasketballMatchConfiguration());

        // Volleyball
        modelBuilder.ApplyConfiguration(new VolleyballTeamConfiguration());
        modelBuilder.ApplyConfiguration(new VolleyballEventConfiguration());
        modelBuilder.ApplyConfiguration(new VolleyballEventParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new VolleyballMatchConfiguration());
        modelBuilder.ApplyConfiguration(new VolleyballSetConfiguration());

        // Beach volleyball
        modelBuilder.ApplyConfiguration(new BeachVolleyballTeamConfiguration());
        modelBuilder.ApplyConfiguration(new BeachVolleyballEventConfiguration());
        modelBuilder.ApplyConfiguration(new BeachVolleyballEventParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new BeachVolleyballMatchConfiguration());
        modelBuilder.ApplyConfiguration(new BeachVolleyballSetConfiguration());

        // Badminton configurations
        modelBuilder.ApplyConfiguration(new BadmintonTeamConfiguration());
        modelBuilder.ApplyConfiguration(new BadmintonEventConfiguration());
        modelBuilder.ApplyConfiguration(new BadmintonEventParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new BadmintonMatchConfiguration());
        modelBuilder.ApplyConfiguration(new BadmintonSetConfiguration());

        // Athletics configurations
        modelBuilder.ApplyConfiguration(new AthleticsEventConfiguration());
        modelBuilder.ApplyConfiguration(new AthleticsResultConfiguration());
        modelBuilder.ApplyConfiguration(new AthleticsEventParticipantConfiguration());

        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
    }
}
