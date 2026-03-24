using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Basketball3x3;

namespace PolytechLeague.Persistence.DbConfigurations.Basketball3x3Configurations;

public class Basketball3x3PlayerConfiguration : IEntityTypeConfiguration<Basketball3x3Player>
{
    public void Configure(EntityTypeBuilder<Basketball3x3Player> builder)
    {
        builder.ToTable("basketball3x3_players");
        
        builder.HasOne(p => p.Team)
            .WithMany(t => t.Players)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
