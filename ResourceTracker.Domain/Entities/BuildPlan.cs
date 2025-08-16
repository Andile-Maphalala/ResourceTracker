
namespace ResourceTracker.Domain.Entities
{
    public class BuildPlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int GameSaveId { get; set; }
        public virtual GameSave GameSave { get; set; }
        public virtual ICollection<BuildPlanComponent> BuildPlanComponents { get; set; }
        public virtual ICollection<BuildPlanQuest> BuildPlanQuests { get; set; }
    }
}
