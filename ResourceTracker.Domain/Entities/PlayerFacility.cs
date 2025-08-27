
namespace ResourceTracker.Domain.Entities
{
    public class PlayerFacility
    {
        public int Id { get; set; }
        public int GameSaveId { get; set; }
        public int ComponentId { get; set; }
        public int QuestId { get; set; }
        public virtual GameSave GameSave { get; set; }
        public virtual Component Component { get; set; }
        public virtual Quest Quest { get; set; }
    }
}
