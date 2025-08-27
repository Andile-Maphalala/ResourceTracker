

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.Data.Configurations
{
    public class BuildPlanQuestConfiguration : IEntityTypeConfiguration<BuildPlanQuest>
    {
        public void Configure(EntityTypeBuilder<BuildPlanQuest> builder)
        {
            builder.ToTable(nameof(BuildPlanQuest));

            // Key
            builder.HasKey(x => new { x.BuildPlanId, x.QuestId });
        }
    }
}
