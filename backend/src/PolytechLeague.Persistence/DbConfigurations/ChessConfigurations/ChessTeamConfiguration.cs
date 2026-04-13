using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Chess;

namespace PolytechLeague.Persistence.DbConfigurations.ChessConfigurations;

public class ChessTeamConfiguration : IEntityTypeConfiguration<ChessTeam>
{
    public void Configure(EntityTypeBuilder<ChessTeam> builder)
    {
        builder.ToTable("chess_teams");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);

        builder.HasOne(t => t.Event)
            .WithMany(e => e.Teams)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
