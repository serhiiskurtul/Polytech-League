using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Football;

namespace PolytechLeague.Persistence.DbConfigurations.FootballConfigurations;

public class FootballGoalConfiguration : IEntityTypeConfiguration<FootballGoal>
{
    public void Configure(EntityTypeBuilder<FootballGoal> builder)
    {
        builder.ToTable("Football_goals");
        
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
