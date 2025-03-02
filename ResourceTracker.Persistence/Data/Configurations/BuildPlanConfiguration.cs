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
    public class BuildPlanConfiguration : IEntityTypeConfiguration<BuildPlan>
    {
        public void Configure(EntityTypeBuilder<BuildPlan> builder)
        {
            builder.ToTable(nameof(BuildPlan));

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

            // Relationships
            builder.HasOne(bp => bp.User)
                .WithMany(u => u.BuildPlans)
                .HasForeignKey(bp => bp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(bp => bp.BuildPlanComponents)
                .WithOne(bpc => bpc.BuildPlan)
                .HasForeignKey(bpc => bpc.BuildPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
