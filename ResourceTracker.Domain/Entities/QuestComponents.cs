

namespace ResourceTracker.Domain.Entities
{
    public class QuestComponents
    {
        public int Id { get; set; }

        public int AmountAquired { get; set; }

        public int ComponentId { get; set; }

        public int QuestId { get; set; }

        public virtual Component Component { get; set; }

        public virtual Quest Quest { get; set; }

    }
}
