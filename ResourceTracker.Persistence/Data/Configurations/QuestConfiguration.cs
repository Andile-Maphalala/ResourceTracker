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
            builder.HasMany(x => x.QuestComponents)
                .WithOne(e => e.Quest)
                .HasForeignKey(e => e.QuestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.PlayerFacilities)
                .WithOne(e => e.Quest)
                .HasForeignKey(e => e.QuestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.BuildPlanQuests)
                .WithOne(e => e.Quest)
                .HasForeignKey(e => e.QuestId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

}
