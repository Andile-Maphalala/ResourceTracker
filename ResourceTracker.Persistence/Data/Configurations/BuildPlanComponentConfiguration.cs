using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Domain.Entities;

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
        }
    }
}
