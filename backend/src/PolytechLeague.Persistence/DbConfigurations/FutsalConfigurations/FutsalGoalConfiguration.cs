using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Futsal;

namespace PolytechLeague.Persistence.DbConfigurations.FutsalConfigurations;

public class FutsalGoalConfiguration : IEntityTypeConfiguration<FutsalGoal>
{
    public void Configure(EntityTypeBuilder<FutsalGoal> builder)
    {
        builder.ToTable("futsal_goals");
        
        builder.HasKey(g => g.Id);
        
        builder.HasOne(g => g.Match)
            .WithMany(m => m.Goals)
            .HasForeignKey(g => g.MatchId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(g => g.Player)
            .WithMany()
            .HasForeignKey(g => g.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.Property(g => g.GoalsCount).IsRequired().HasDefaultValue(1);
    }
}
