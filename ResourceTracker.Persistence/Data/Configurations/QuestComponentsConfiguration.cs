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
    public class QuestComponentsConfiguration : IEntityTypeConfiguration<QuestComponents>
    {
        public void Configure(EntityTypeBuilder<QuestComponents> builder)
        {
            builder.ToTable(nameof(QuestComponents));

            // Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(e => e.Quantity)
                .IsRequired();

            // Relationships
            builder.HasOne(qc => qc.Quest)
                .WithMany(q => q.QuestComponents)
                .HasForeignKey(qc => qc.QuestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(qc => qc.Component)
                .WithMany(c => c.QuestComponents)
                .HasForeignKey(qc => qc.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
