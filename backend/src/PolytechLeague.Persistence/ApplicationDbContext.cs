using PolytechLeague.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.PostgreSQL;
using PolytechLeague.Domain.Entities.Chess;
using PolytechLeague.Infrastructure.Persistence.DbConfiguration;
using PolytechLeague.Infrastructure.Persistence.DbConfiguration.ChessConfigurations;
using PolytechLeague.Domain.Entities.Shared;
using PolytechLeague.Domain.Entities.Checkers;
using PolytechLeague.Infrastructure.Persistence.DbConfiguration.CheckersConfigurations;
using PolytechLeague.Domain.Entities.TableTennis;
using PolytechLeague.Domain.Entities.TableTennis.Enums;
using PolytechLeague.Infrastructure.Persistence.DbConfiguration.TableTennisConfigurations;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<UserRole>();
        modelBuilder.HasPostgresEnum<GameResult>();
        modelBuilder.HasPostgresEnum<CompetitionLevel>();
        modelBuilder.HasPostgresEnum<ChessSystem>();
        modelBuilder.HasPostgresEnum<CheckersSystem>();
        modelBuilder.HasPostgresEnum<CheckersVariant>();
        modelBuilder.HasPostgresEnum<TableTennisHand>();
        modelBuilder.HasPostgresEnum<TableTennisTournamentType>();
        modelBuilder.HasPostgresEnum<TableTennisCategory>();

        modelBuilder.Entity<Person>().UseTptMappingStrategy();
        modelBuilder.Entity<Event>().UseTptMappingStrategy();

        //General configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new EventPhotosConfiguration());

        // Chess configurations
        modelBuilder.ApplyConfiguration(new PlayerConfiguration());
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

        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
    }
}
