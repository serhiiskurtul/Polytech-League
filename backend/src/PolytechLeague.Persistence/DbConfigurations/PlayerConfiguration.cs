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

        // Checkers
        builder.Property(x => x.CheckersRating).HasColumnName("checkers_rating");

        // Table Tennis
        builder.Property(x => x.TableTennisRating).HasColumnName("table_tennis_rating");
        builder.Property(x => x.TableTennisHand).HasColumnName("table_tennis_hand");

        // Armwrestling
        builder.Property(x => x.ArmwrestlingWeight).HasColumnName("armwrestling_weight");
        builder.Property(x => x.ArmwrestlingPreferredWeightClass).HasColumnName("armwrestling_preferred_weight_class");

        // Futsal
        builder.Property(x => x.FutsalPosition).HasColumnName("futsal_position");
        builder.Property(x => x.FutsalShirtNumber).HasColumnName("futsal_shirt_number");
        builder.Property(x => x.FutsalTeamId).HasColumnName("futsal_team_id");

        // Volleyball
        builder.Property(x => x.VolleyballPosition).HasColumnName("volleyball_position");
        builder.Property(x => x.VolleyballShirtNumber).HasColumnName("volleyball_shirt_number");
        builder.Property(x => x.VolleyballTeamId).HasColumnName("volleyball_team_id");

        // Basketball
        builder.Property(x => x.BasketballShirtNumber).HasColumnName("basketball_shirt_number");
        builder.Property(x => x.BasketballTeamId).HasColumnName("basketball_team_id");

        // Beach Volleyball
        builder.Property(x => x.BeachVolleyballShirtNumber).HasColumnName("beach_volleyball_shirt_number");
        builder.Property(x => x.BeachVolleyballTeamId).HasColumnName("beach_volleyball_team_id");

        // Relationships (Optional navigations could be added here if needed, 
        // but they are usually handled by EF conventions or explicitly in Team configs)
    }
}
