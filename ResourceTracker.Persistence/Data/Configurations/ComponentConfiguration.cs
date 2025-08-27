using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.Data.Configurations
{
    public class ComponentConfiguration : IEntityTypeConfiguration<Component>
    {
        public void Configure(EntityTypeBuilder<Component> builder)
        {
            builder.ToTable(nameof(Component));

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
            builder.HasMany(x => x.Recipes)
                .WithOne(e => e.Component)
                .HasForeignKey(e => e.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.RecipeComponents)
                .WithOne(e => e.Component)
                .HasForeignKey(e => e.ComponentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.QuestComponents)
                .WithOne(e => e.Component)
                .HasForeignKey(e => e.ComponentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.BuildPlanComponents)
               .WithOne(e => e.Component)
               .HasForeignKey(e => e.ComponentId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.PlayerFacilities)
                .WithOne(e => e.Component)
                .HasForeignKey(e => e.ComponentId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }

}
