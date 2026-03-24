using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Volleyball;

namespace PolytechLeague.Persistence.DbConfigurations.VolleyballConfigurations;

public class VolleyballPlayerConfiguration : IEntityTypeConfiguration<VolleyballPlayer>
{
    public void Configure(EntityTypeBuilder<VolleyballPlayer> builder)
    {
        builder.ToTable("volleyball_players");

        builder.HasOne(p => p.Team)
            .WithMany(t => t.Players)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
