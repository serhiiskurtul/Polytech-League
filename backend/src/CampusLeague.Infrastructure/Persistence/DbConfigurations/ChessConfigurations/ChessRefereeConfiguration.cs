using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CampusLeague.Domain.Entities.Chess;

namespace CampusLeague.Persistence.DbConfiguration.ChessConfigurations;

public class ChessRefereeConfiguration : IEntityTypeConfiguration<ChessReferee>
{
    public void Configure(EntityTypeBuilder<ChessReferee> builder)
    {
        builder.ToTable("chess_referees");
    }
}
