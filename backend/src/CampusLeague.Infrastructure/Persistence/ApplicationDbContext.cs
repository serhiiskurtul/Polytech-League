using CampusLeague.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.PostgreSQL;
using CampusLeague.Domain.Entities.Chess;

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
        modelBuilder.HasPostgresEnum<GameResult>();
        modelBuilder.HasPostgresEnum<CompetitionLevel>();
        modelBuilder.HasPostgresEnum<ChessSystem>();

        modelBuilder.ApplyConfiguration(new UserConfiguration());

        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
    }
}
