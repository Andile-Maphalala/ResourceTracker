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
        }
    }
}
