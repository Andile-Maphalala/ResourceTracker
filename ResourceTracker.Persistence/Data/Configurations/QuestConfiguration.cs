using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.Data.Configurations
{
    public class QuestConfiguration : IEntityTypeConfiguration<Quest>
    {
        public void Configure(EntityTypeBuilder<Quest> builder)
        {
            builder.ToTable(nameof(Quest));

            // Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);

            builder.Property(e => e.Location)
                .HasMaxLength(250)
                .IsUnicode(false);

            // Relationships
            builder.HasMany(q => q.QuestComponents)
                .WithOne(qc => qc.Quest)
                .HasForeignKey(qc => qc.QuestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(q => q.GameSave)
                .WithMany(g => g.Quests)
                .HasForeignKey(q => q.GameSaveId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(q => q.Picture)
                .WithMany(p => p.Quests)
                .HasForeignKey(q => q.PictureId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }

}
