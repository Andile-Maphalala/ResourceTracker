using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.Data.Configurations
{
    public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.ToTable(nameof(Recipe));

            // Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(e => e.AmountMade)
                .IsRequired();

            // Minimum level of RequiredFacility needed to craft this. Meaningless when
            // RequiredFacilityId is null. Default 1 leaves recipes with no level (Subnautica) as
            // they were, and any owned facility satisfies it.
            builder.Property(e => e.RequiredFacilityLevel)
                .IsRequired()
                .HasDefaultValue(1);

            // Relationships
            builder.HasMany(x => x.RecipeComponents)
                .WithOne(e => e.Recipe)
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
