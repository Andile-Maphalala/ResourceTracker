using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.Data.Configurations
{
    public class RecipeComponentConfiguration : IEntityTypeConfiguration<RecipeComponent>
    {
        public void Configure(EntityTypeBuilder<RecipeComponent> builder)
        {
            builder.ToTable(nameof(RecipeComponent));

            // Key
            builder.HasKey(e => new { e.RecipeId, e.ComponentId });

            // Properties
            builder.Property(e => e.AmountRequired)
                .IsRequired();
        }
    }

}
