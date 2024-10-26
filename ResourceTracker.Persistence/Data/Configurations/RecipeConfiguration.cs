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

            // Relationships
            builder.HasOne(r => r.Component)
                .WithMany(c => c.Recipes)
                .HasForeignKey(r => r.ComponentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.RecipeComponents)
                .WithOne(c => c.Recipe)
                .HasForeignKey(r => r.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
