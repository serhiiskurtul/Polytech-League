using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.TableTennis;

namespace PolytechLeague.Persistence.DbConfigurations.TableTennisConfigurations;

public class TableTennisEventConfiguration : IEntityTypeConfiguration<TableTennisEvent>
{
    public void Configure(EntityTypeBuilder<TableTennisEvent> builder)
    {
        builder.ToTable("table_tennis_events");

        builder.HasIndex(x => x.TournamentType).HasDatabaseName("IX_tt_events_type");
        builder.HasIndex(x => x.Category).HasDatabaseName("IX_tt_events_category");

        builder.Property(x => x.TournamentType)
            .HasColumnName("tournament_type")
            .IsRequired();

        builder.Property(x => x.Category)
            .HasColumnName("category")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(x => x.FirstPlaceTeamId)
            .HasColumnName("first_place_team_id");

        builder.Property(x => x.SecondPlaceTeamId)
            .HasColumnName("second_place_team_id");

        builder.Property(x => x.ThirdPlaceTeamId)
            .HasColumnName("third_place_team_id");

        builder.HasMany(x => x.Matches)
            .WithOne(m => m.Tournament)
            .HasForeignKey(m => m.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Participants)
            .WithOne(p => p.Tournament)
            .HasForeignKey(p => p.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Teams)
            .WithOne(t => t.Event)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.FirstPlaceTeam)
            .WithMany()
            .HasForeignKey(x => x.FirstPlaceTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SecondPlaceTeam)
            .WithMany()
            .HasForeignKey(x => x.SecondPlaceTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ThirdPlaceTeam)
            .WithMany()
            .HasForeignKey(x => x.ThirdPlaceTeamId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
