

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.Data.Configurations
{
    public class PictureConfiguration : IEntityTypeConfiguration<Picture>
    {
        public void Configure(EntityTypeBuilder<Picture> builder)
        {
            builder.ToTable(nameof(Picture));

            // Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(e => e.Path)
                .IsRequired()
                .HasMaxLength(500)
                .IsUnicode(false);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.Property(e => e.ContentType)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.AltText)
                .HasMaxLength(200)
                .IsUnicode(false);

            builder.Property(e => e.CreatedDate)
                .IsRequired();

            //Relationships
            builder.HasMany(x => x.Games)
              .WithOne(e => e.Picture)
              .HasForeignKey(e => e.PictureId)
              .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.Quests)
              .WithOne(e => e.Picture)
              .HasForeignKey(e => e.PictureId)
              .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.Components)
              .WithOne(e => e.Picture)
              .HasForeignKey(e => e.PictureId)
              .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
