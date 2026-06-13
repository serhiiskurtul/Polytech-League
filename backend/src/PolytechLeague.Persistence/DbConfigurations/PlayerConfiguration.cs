using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Persistence.DbConfigurations;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("players");

        builder.Property(x => x.City)
            .HasColumnName("city")
            .IsRequired();

        builder.Property(x => x.Group)
            .HasColumnName("group")
            .IsRequired();

        builder.Property(x => x.StudentId)
            .HasColumnName("student_id");

        builder.HasIndex(x => x.StudentId)
            .IsUnique();

        // Chess
        builder.Property(x => x.ChessRating).HasColumnName("chess_rating");
        builder.Property(x => x.ChessFideId).HasColumnName("chess_fide_id");
        builder.Property(x => x.ChessTitle).HasColumnName("chess_title");

        builder.HasMany(x => x.ChessTeams)
            .WithMany(t => t.Players)
            .UsingEntity(j => j.ToTable("chess_team_players"));

        // Checkers
        builder.Property(x => x.CheckersRating).HasColumnName("checkers_rating");

        builder.HasMany(x => x.CheckersTeams)
            .WithMany(t => t.Players)
            .UsingEntity(j => j.ToTable("checkers_team_players"));

        // Table Tennis
        builder.Property(x => x.TableTennisRating).HasColumnName("table_tennis_rating");
        builder.Property(x => x.TableTennisHand).HasColumnName("table_tennis_hand");

        // Armwrestling
        builder.Property(x => x.ArmwrestlingWeight).HasColumnName("armwrestling_weight");
        builder.Property(x => x.ArmwrestlingPreferredWeightClass).HasColumnName("armwrestling_preferred_weight_class");

        // Football
        builder.Property(x => x.FootballPosition).HasColumnName("Football_position");
        builder.Property(x => x.FootballShirtNumber).HasColumnName("Football_shirt_number");
        builder.Property(x => x.FootballTeamId).HasColumnName("Football_team_id");

        // Volleyball
        builder.Property(x => x.VolleyballPosition).HasColumnName("volleyball_position");
        builder.Property(x => x.VolleyballShirtNumber).HasColumnName("volleyball_shirt_number");

        builder.HasMany(x => x.VolleyballTeams)
            .WithMany(t => t.Players)
            .UsingEntity(j => j.ToTable("volleyball_team_players"));

        // Basketball
        builder.Property(x => x.BasketballShirtNumber).HasColumnName("basketball_shirt_number");

        builder.HasMany(x => x.BasketballTeams)
            .WithMany(t => t.Players)
            .UsingEntity(j => j.ToTable("basketball_team_players"));

        // Beach Volleyball
        builder.Property(x => x.BeachVolleyballShirtNumber).HasColumnName("beach_volleyball_shirt_number");

        builder.HasMany(x => x.BeachVolleyballTeams)
            .WithMany(t => t.Players)
            .UsingEntity(j => j.ToTable("beach_volleyball_team_players"));

        // Social & Bio
        builder.Property(x => x.Bio).HasColumnName("Bio");
        builder.Property(x => x.InstagramUrl).HasColumnName("InstagramUrl");

        builder.HasOne(x => x.College)
            .WithMany()
            .HasForeignKey(x => x.CollegeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
