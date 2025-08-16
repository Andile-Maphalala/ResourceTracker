
namespace ResourceTracker.Domain.Entities
{
    public class BuildPlanQuest
    {
        public int BuildPlanId { get; set; }
        public int QuestId { get; set; }
        public virtual BuildPlan BuildPlan { get; set; }
        public virtual Quest Quest { get; set; }
    }
}
