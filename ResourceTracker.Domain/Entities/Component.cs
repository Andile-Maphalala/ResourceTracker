using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Domain.Entities
{
    public class Component
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }
        public virtual ICollection<Recipe> ParentRecipes { get; set; }
        public virtual ICollection<QuestComponents> QuestComponents { get; set; }
    }
}
