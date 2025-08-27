
namespace ResourceTracker.Domain.Entities
{
    public class GameSave
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public virtual User User { get; set; }
        public virtual Game Game { get; set; }
        public virtual ICollection<Quest> Quests { get; set; }
        public virtual ICollection<BuildPlan> BuildPlans { get; set; }
        public virtual ICollection<PlayerFacility> PlayerFacilities { get; set; }
    }
}
