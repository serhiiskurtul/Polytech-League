using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Infrastructure.Persistence.DbConfiguration;

public class EventPhotosConfiguration : IEntityTypeConfiguration<EventPhotos>
{
    public void Configure(EntityTypeBuilder<EventPhotos> builder)
    {
        builder.ToTable("event_photos");

        builder.HasIndex(x => x.EventId).HasDatabaseName("IX_event_photos_event_id");

        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Url)
            .HasColumnName("url")
            .IsRequired();

        builder.Property(x => x.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.HasOne(x => x.Event)
            .WithMany(x => x.Photos)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
