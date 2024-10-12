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
    public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.ToTable(nameof(Resource));

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
            builder.HasMany(r => r.Recipes)
                .WithOne(recipe => recipe.Resource)
                .HasForeignKey(recipe => recipe.ResourceId);

            builder.HasMany(r => r.ResourceCollections)
                .WithOne(rc => rc.Resource)
                .HasForeignKey(rc => rc.ResourceId);
        }
    }

}
