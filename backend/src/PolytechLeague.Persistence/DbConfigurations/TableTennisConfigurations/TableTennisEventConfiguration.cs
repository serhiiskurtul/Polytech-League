using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.TableTennis;

namespace PolytechLeague.Infrastructure.Persistence.DbConfiguration.TableTennisConfigurations;

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

        builder.HasMany(x => x.Matches)
            .WithOne(m => m.Tournament)
            .HasForeignKey(m => m.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Participants)
            .WithOne(p => p.Tournament)
            .HasForeignKey(p => p.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
