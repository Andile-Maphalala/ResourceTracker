using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            builder.Property(e => e.Image)
                .IsRequired(false);

            // Relationships
            builder.HasOne(q => q.User)
                .WithMany(u => u.Quests)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.QuestComponents)
                .WithOne(qc => qc.Quest)
                .HasForeignKey(qc => qc.QuestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.ResourceCollections)
                .WithOne(rc => rc.Quest)
                .HasForeignKey(rc => rc.QuestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
