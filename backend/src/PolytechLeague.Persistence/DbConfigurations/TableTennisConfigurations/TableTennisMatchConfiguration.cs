using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.TableTennis;

namespace PolytechLeague.Persistence.DbConfigurations.TableTennisConfigurations;

public class TableTennisMatchConfiguration : IEntityTypeConfiguration<TableTennisMatch>
{
    public void Configure(EntityTypeBuilder<TableTennisMatch> builder)
    {
        builder.ToTable("table_tennis_matches");

        // ---------- Columns ----------
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.TournamentId)
            .HasColumnName("tournament_id")
            .IsRequired();

        builder.Property(x => x.Player1Id)
            .HasColumnName("player1_id");

        builder.Property(x => x.Player2Id)
            .HasColumnName("player2_id");

        builder.Property(x => x.HomeTeamId)
            .HasColumnName("home_team_id");

        builder.Property(x => x.AwayTeamId)
            .HasColumnName("away_team_id");

        builder.Property(x => x.WinnerId)
            .HasColumnName("winner_id");

        builder.Property(x => x.WinnerTeamId)
            .HasColumnName("winner_team_id");

        builder.Property(x => x.HomeTeamScore)
            .HasColumnName("home_team_score");

        builder.Property(x => x.AwayTeamScore)
            .HasColumnName("away_team_score");

        builder.Property(x => x.Stage)
            .HasColumnName("stage");

        builder.Property(x => x.TableNumber)
            .HasColumnName("table_number")
            .HasDefaultValue(0);

        builder.Property(x => x.SetsToWin)
            .HasColumnName("sets_to_win")
            .HasDefaultValue(3);

        // ---------- Indexes ----------
        builder.HasIndex(x => x.TournamentId)
            .HasDatabaseName("IX_tt_matches_tournament_id");

        // ---------- Relationships ----------

        builder.HasOne(m => m.Tournament)
            .WithMany(t => t.Matches)
            .HasForeignKey(m => m.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Player1)
            .WithMany()
            .HasForeignKey(m => m.Player1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Player2)
            .WithMany()
            .HasForeignKey(m => m.Player2Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.HomeTeam)
            .WithMany()
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.AwayTeam)
            .WithMany()
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Winner)
            .WithMany()
            .HasForeignKey(m => m.WinnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.WinnerTeam)
            .WithMany()
            .HasForeignKey(m => m.WinnerTeamId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(m => m.Sets)
            .WithOne(s => s.Match)
            .HasForeignKey(s => s.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
