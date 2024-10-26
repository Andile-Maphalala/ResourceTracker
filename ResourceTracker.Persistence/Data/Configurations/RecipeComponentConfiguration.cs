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

            // Relationships
            builder.HasOne(rc => rc.Recipe)
                .WithMany(q => q.RecipeComponents)
                .HasForeignKey(rc => rc.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rc => rc.Component)
                .WithMany(r => r.RecipeComponents)
                .HasForeignKey(rc => rc.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
