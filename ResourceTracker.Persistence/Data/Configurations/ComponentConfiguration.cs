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
            builder.HasMany(c => c.Recipes)
                .WithOne(r => r.Component)
                .HasForeignKey(r => r.ComponentId);

            builder.HasMany(c => c.RecipeComponents)
                .WithOne(r => r.Component)
                .HasForeignKey(r => r.ComponentId);

            builder.HasMany(c => c.QuestComponents)
                .WithOne(qc => qc.Component)
                .HasForeignKey(qc => qc.ComponentId);

            builder.HasMany(c => c.BuildPlanComponents)
               .WithOne(bpc => bpc.Component)
               .HasForeignKey(bpc => bpc.ComponentId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(q => q.Game)
                .WithMany(g => g.Components)
                .HasForeignKey(q => q.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(q => q.Picture)
                .WithMany(p => p.Components)
                .HasForeignKey(q => q.PictureId)
                .OnDelete(DeleteBehavior.SetNull);


        }
    }

}
