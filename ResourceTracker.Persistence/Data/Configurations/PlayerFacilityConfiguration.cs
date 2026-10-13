using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.Data.Configurations
{
    public class PlayerFacilityConfiguration : IEntityTypeConfiguration<PlayerFacility>
    {
        public void Configure(EntityTypeBuilder<PlayerFacility> builder)
        {
            builder.ToTable(nameof(PlayerFacility));

            // Key
            builder.HasKey(x => x.Id);

            // The level of the facility at this location. 1 is a freshly built one. Availability is
            // checked with >= against Recipe.RequiredFacilityLevel, so a higher level satisfies a
            // lower ask; the default only matters for rows written outside the application.
            builder.Property(x => x.Level)
                .IsRequired()
                .HasDefaultValue(1);
        }
    }
}
