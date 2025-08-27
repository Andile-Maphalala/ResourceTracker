
namespace ResourceTracker.Domain.Entities
{
    public class Quest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int? PictureId { get; set; }
        public int GameSaveId { get; set; }
        public virtual Picture Picture { get; set; }
        public virtual GameSave GameSave { get; set; }
        public virtual ICollection<QuestComponents> QuestComponents { get; set; }
        public virtual ICollection<BuildPlanQuest> BuildPlanQuests { get; set; }
        public virtual ICollection<PlayerFacility> PlayerFacilities { get; set; }

    }
}
