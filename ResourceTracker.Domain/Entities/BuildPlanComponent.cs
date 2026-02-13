
namespace ResourceTracker.Domain.Entities
{
    public class BuildPlanComponent
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public int QuantityNeeded { get; set; }
        public int ComponentId { get; set; }
        public int BuildPlanId { get; set; }
        public virtual Component Component { get; set; }
        public virtual BuildPlan BuildPlan { get; set; }
    }
}
