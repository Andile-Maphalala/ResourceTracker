using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResourceTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Persistence.Data.Configurations
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable(nameof(Game));

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
            builder.HasMany(bp => bp.Quests)
                .WithOne(bpc => bpc.Game)
                .HasForeignKey(bpc => bpc.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(bp => bp.BuildPlans)
                .WithOne(bpc => bpc.Game)
                .HasForeignKey(bpc => bpc.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(bp => bp.Components)
                .WithOne(bpc => bpc.Game)
                .HasForeignKey(bpc => bpc.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(q => q.Picture)
                .WithMany(p => p.Games)
                .HasForeignKey(q => q.PictureId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
