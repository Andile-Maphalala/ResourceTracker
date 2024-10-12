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
    public class ResourceCollectionConfiguration : IEntityTypeConfiguration<ResourceCollection>
    {
        public void Configure(EntityTypeBuilder<ResourceCollection> builder)
        {
            builder.ToTable(nameof(ResourceCollection));

            // Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(e => e.ItemsGathered)
                .IsRequired();

            // Relationships
            builder.HasOne(rc => rc.Quest)
                .WithMany(q => q.ResourceCollections)
                .HasForeignKey(rc => rc.QuestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rc => rc.Resource)
                .WithMany(r => r.ResourceCollections)
                .HasForeignKey(rc => rc.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
