
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.Data.Configurations
{
    public class GameSaveConfiguration : IEntityTypeConfiguration<GameSave>
    {
        public void Configure(EntityTypeBuilder<GameSave> builder)
        {
            builder.ToTable(nameof(GameSave));
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
            builder.HasMany(x => x.Quests)
                .WithOne(e => e.GameSave)
                .HasForeignKey(e => e.GameSaveId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.BuildPlans)
                .WithOne(e => e.GameSave)
                .HasForeignKey(e => e.GameSaveId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.PlayerFacilities)
                .WithOne(e => e.GameSave)
                .HasForeignKey(e => e.GameSaveId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
