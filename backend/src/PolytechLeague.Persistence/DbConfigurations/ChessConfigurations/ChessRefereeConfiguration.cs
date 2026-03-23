using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Chess;

namespace PolytechLeague.Persistence.DbConfigurations.ChessConfigurations;

public class ChessRefereeConfiguration : IEntityTypeConfiguration<ChessReferee>
{
    public void Configure(EntityTypeBuilder<ChessReferee> builder)
    {
        builder.ToTable("chess_referees");
    }
}
