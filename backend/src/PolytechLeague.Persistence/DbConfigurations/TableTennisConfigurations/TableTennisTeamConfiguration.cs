using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.TableTennis;

namespace PolytechLeague.Persistence.DbConfigurations.TableTennisConfigurations;

public class TableTennisTeamConfiguration : IEntityTypeConfiguration<TableTennisTeam>
{
    public void Configure(EntityTypeBuilder<TableTennisTeam> builder)
    {
        builder.ToTable("table_tennis_teams");

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name").IsRequired();
        builder.Property(x => x.EventId).HasColumnName("event_id").IsRequired();
        builder.Property(x => x.Points).HasColumnName("points").HasDefaultValue(0);
        builder.Property(x => x.Rank).HasColumnName("rank").HasDefaultValue(0);

        builder.HasOne(x => x.Event)
            .WithMany(x => x.Teams)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Players)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "table_tennis_team_players",
                j => j.HasOne<PolytechLeague.Domain.Entities.Player>().WithMany().HasForeignKey("player_id"),
                j => j.HasOne<TableTennisTeam>().WithMany().HasForeignKey("team_id")
            );
    }
}
