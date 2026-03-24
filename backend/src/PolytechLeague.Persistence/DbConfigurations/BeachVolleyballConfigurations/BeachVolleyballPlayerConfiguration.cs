using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.BeachVolleyball;

namespace PolytechLeague.Persistence.DbConfigurations.BeachVolleyballConfigurations;

public class BeachVolleyballPlayerConfiguration : IEntityTypeConfiguration<BeachVolleyballPlayer>
{
    public void Configure(EntityTypeBuilder<BeachVolleyballPlayer> builder)
    {
        builder.ToTable("beach_volleyball_players");

        builder.HasOne(p => p.Team)
            .WithMany(t => t.Players)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
