using CampusLeague.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.PostgreSQL;
using CampusLeague.Domain.Entities.Chess;
using CampusLeague.Persistence.DbConfiguration;
using CampusLeague.Persistence.DbConfiguration.ChessConfigurations;

namespace CampusLeague.Persistence.Database;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<UserRole>();
        modelBuilder.HasPostgresEnum<ChessGameResult>();
        modelBuilder.HasPostgresEnum<CompetitionLevel>();
        modelBuilder.HasPostgresEnum<ChessSystem>();

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

        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
    }
}
