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
using PolytechLeague.Domain.Entities.Futsal;
using PolytechLeague.Domain.Entities.Futsal.Enums;
using PolytechLeague.Persistence.DbConfigurations.FutsalConfigurations;
using PolytechLeague.Domain.Entities.Basketball3x3;
using PolytechLeague.Domain.Entities.Basketball3x3.Enums;
using PolytechLeague.Persistence.DbConfigurations.Basketball3x3Configurations;

namespace PolytechLeague.Persistence.Database;

public class ApplicationDbContext : DbContext   
{
    public ApplicationDbContext() { } //used for Service Tests
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<EventPhotos> EventPhotos { get; set; }
    public DbSet<ChessEventParticipant> ChessEventParticipants { get; set; }
    public DbSet<ChessEvent> ChessEvents { get; set; }
    public DbSet<ChessGame> ChessGames { get; set; }
    public DbSet<ChessPlayer> ChessPlayers { get; set; }
    public DbSet<ChessReferee> ChessReferees { get; set; }
    public DbSet<CheckersGame> CheckersGames { get; set; }
    public DbSet<CheckersPlayer> CheckersPlayers { get; set; }
    public DbSet<CheckersEventParticipant> CheckersEventParticipants { get; set; }
    public DbSet<CheckersReferee> CheckersReferees { get; set; }
    public DbSet<CheckersEvent> CheckersEvents { get; set; }
    public DbSet<TableTennisPlayer> TableTennisPlayers { get; set; }
    public DbSet<TableTennisEvent> TableTennisEvents { get; set; }
    public DbSet<TableTennisEventParticipant> TableTennisEventParticipants { get; set; }
    public DbSet<TableTennisMatch> TableTennisMatches { get; set; }
    public DbSet<TableTennisSet> TableTennisSets { get; set; }

    // Armwrestling
    public DbSet<ArmwrestlingPlayer> ArmwrestlingPlayers { get; set; }
    public DbSet<ArmwrestlingEvent> ArmwrestlingEvents { get; set; }
    public DbSet<ArmwrestlingMatch> ArmwrestlingMatches { get; set; }

    // Futsal
    public DbSet<FutsalTeam> FutsalTeams { get; set; }
    public DbSet<FutsalPlayer> FutsalPlayers { get; set; }
    public DbSet<FutsalEvent> FutsalEvents { get; set; }
    public DbSet<FutsalMatch> FutsalMatches { get; set; }

    // Basketball 3x3
    public DbSet<Basketball3x3Team> Basketball3x3Teams { get; set; }
    public DbSet<Basketball3x3Player> Basketball3x3Players { get; set; }
    public DbSet<Basketball3x3Event> Basketball3x3Events { get; set; }
    public DbSet<Basketball3x3Match> Basketball3x3Matches { get; set; }

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
            .HasPostgresEnum<TableTennisCategory>()
            .HasPostgresEnum<ArmwrestlingHand>()
            .HasPostgresEnum<ArmwrestlingCategory>()
            .HasPostgresEnum<ArmwrestlingWeightClass>()
            .HasPostgresEnum<FutsalPosition>()
            .HasPostgresEnum<Basketball3x3TournamentType>();

        modelBuilder.Entity<Person>().UseTptMappingStrategy();
        modelBuilder.Entity<Event>().UseTptMappingStrategy();

        //General configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new EventPhotosConfiguration());

        // Chess configurations
        modelBuilder.ApplyConfiguration(new ChessPlayerConfiguration());
        modelBuilder.ApplyConfiguration(new ChessRefereeConfiguration());
        modelBuilder.ApplyConfiguration(new ChessEventConfiguration());
        modelBuilder.ApplyConfiguration(new ChessEventParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new ChessGameConfiguration());

        // Checkers configurations
        modelBuilder.ApplyConfiguration(new CheckersPlayerConfiguration());
        modelBuilder.ApplyConfiguration(new CheckersRefereeConfiguration());
        modelBuilder.ApplyConfiguration(new CheckersEventConfiguration());
        modelBuilder.ApplyConfiguration(new CheckersEventParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new CheckersGameConfiguration());

        // Table Tennis configurations
        modelBuilder.ApplyConfiguration(new TableTennisPlayerConfiguration());
        modelBuilder.ApplyConfiguration(new TableTennisEventConfiguration());
        modelBuilder.ApplyConfiguration(new TableTennisEventParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new TableTennisMatchConfiguration());
        modelBuilder.ApplyConfiguration(new TableTennisSetConfiguration());

        // Armwrestling configurations
        modelBuilder.ApplyConfiguration(new ArmwrestlingPlayerConfiguration());
        modelBuilder.ApplyConfiguration(new ArmwrestlingEventConfiguration());
        modelBuilder.ApplyConfiguration(new ArmwrestlingMatchConfiguration());

        // Futsal configurations
        modelBuilder.ApplyConfiguration(new FutsalTeamConfiguration());
        modelBuilder.ApplyConfiguration(new FutsalPlayerConfiguration());
        modelBuilder.ApplyConfiguration(new FutsalEventConfiguration());
        modelBuilder.ApplyConfiguration(new FutsalMatchConfiguration());

        // Basketball 3x3 configurations
        modelBuilder.ApplyConfiguration(new Basketball3x3TeamConfiguration());
        modelBuilder.ApplyConfiguration(new Basketball3x3PlayerConfiguration());
        modelBuilder.ApplyConfiguration(new Basketball3x3EventConfiguration());
        modelBuilder.ApplyConfiguration(new Basketball3x3MatchConfiguration());

        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
    }
}
