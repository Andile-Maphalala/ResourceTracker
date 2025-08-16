

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

            //relationships
            builder.HasOne(x => x.BuildPlan)
                .WithMany(x => x.BuildPlanQuests)
                .HasForeignKey(x => x.BuildPlanId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Quest)
               .WithMany(x => x.BuildPlanQuests)
               .HasForeignKey(x => x.QuestId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
