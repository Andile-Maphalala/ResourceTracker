using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.Data.Configurations
{
    public class QuestComponentsConfiguration : IEntityTypeConfiguration<QuestComponents>
    {
        public void Configure(EntityTypeBuilder<QuestComponents> builder)
        {
            builder.ToTable(nameof(QuestComponents));

            // Key
            builder.HasKey(x => x.Id);

            builder.Property(e => e.AmountAquired)
                .IsRequired();
        }
    }

}
