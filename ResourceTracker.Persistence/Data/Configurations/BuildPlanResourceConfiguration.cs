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
    public class BuildPlanResourceConfiguration : IEntityTypeConfiguration<BuildPlanResource>
    {
        public void Configure(EntityTypeBuilder<BuildPlanResource> builder)
        {
            builder.ToTable(nameof(BuildPlanResource));

            // Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(e => e.TotalQuantityNeeded)
                .IsRequired();

            builder.Property(e => e.QuantityGathered)
                .IsRequired();

            // Relationships
            builder.HasOne(bpr => bpr.BuildPlan)
                .WithMany(bp => bp.BuildPlanResources)
                .HasForeignKey(bpr => bpr.BuildPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bpr => bpr.Component)
                .WithMany(c => c.BuildPlanResources)
                .HasForeignKey(bpr => bpr.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bpr => bpr.SourceComponent)
                .WithMany()
                .HasForeignKey(bpr => bpr.SourceComponentId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete for source component
        }
    }
}
