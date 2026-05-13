using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Football;

namespace PolytechLeague.Persistence.DbConfigurations.FootballConfigurations;

public class FootballCardConfiguration : IEntityTypeConfiguration<FootballCard>
{
    public void Configure(EntityTypeBuilder<FootballCard> builder)
    {
        builder.ToTable("Football_cards");
        
        builder.HasKey(c => c.Id);
        
        builder.HasOne(c => c.Match)
            .WithMany(m => m.Cards)
            .HasForeignKey(c => c.MatchId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(c => c.Player)
            .WithMany()
            .HasForeignKey(c => c.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.Property(c => c.Type).IsRequired();
    }
}
