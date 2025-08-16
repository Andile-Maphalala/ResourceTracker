using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Domain.Entities;


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
            builder.HasMany(bp => bp.BuildPlanComponents)
                .WithOne(bpc => bpc.BuildPlan)
                .HasForeignKey(bpc => bpc.BuildPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(q => q.GameSave)
                .WithMany(g => g.BuildPlans)
                .HasForeignKey(q => q.GameSaveId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.BuildPlanQuests)
               .WithOne(x => x.BuildPlan)
               .HasForeignKey(bpr => bpr.BuildPlanId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
