using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Persistence.DbConfigurations;

public class UserEventMediaConfiguration : IEntityTypeConfiguration<UserEventMedia>
{
    public void Configure(EntityTypeBuilder<UserEventMedia> builder)
    {
        builder.ToTable("user_event_media");

        builder.HasIndex(x => x.EventId).HasDatabaseName("IX_user_event_media_event_id");
        builder.HasIndex(x => x.UserId).HasDatabaseName("IX_user_event_media_user_id");

        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Url)
            .HasColumnName("url")
            .IsRequired();

        builder.Property(x => x.Type)
            .HasColumnName("type")
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasColumnName("comment");

        builder.Property(x => x.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasOne(x => x.Event)
            .WithMany(x => x.UserMedia)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
