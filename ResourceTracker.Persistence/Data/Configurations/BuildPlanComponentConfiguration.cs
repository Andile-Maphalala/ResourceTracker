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
    public class BuildPlanComponentConfiguration : IEntityTypeConfiguration<BuildPlanComponent>
    {
        public void Configure(EntityTypeBuilder<BuildPlanComponent> builder)
        {
            builder.ToTable(nameof(BuildPlanComponent));

            // Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(e => e.Order)
                .IsRequired();

            builder.Property(e => e.QuantityNeeded)
                .IsRequired();

            // Relationships
            builder.HasOne(bpc => bpc.BuildPlan)
                .WithMany(bp => bp.BuildPlanComponents)
                .HasForeignKey(bpc => bpc.BuildPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bpc => bpc.Component)
                .WithMany(c => c.BuildPlanComponents)
                .HasForeignKey(bpc => bpc.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
