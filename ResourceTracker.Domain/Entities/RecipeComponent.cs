
namespace ResourceTracker.Domain.Entities
{
    public class RecipeComponent
    {
        public int RecipeId { get; set; }
        public int ComponentId { get; set; }
        public int AmountRequired { get; set; }
        public virtual Recipe Recipe { get; set; }
        public virtual Component Component { get; set; }
    }
}
