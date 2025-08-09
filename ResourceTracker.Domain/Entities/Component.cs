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

        public int Type { get; set; }

        public int GameId { get; set; }

        public int? PictureId { get; set; }

        public  virtual Game Game { get; set; }
        public virtual Picture Picture { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; }
        public virtual ICollection<RecipeComponent> RecipeComponents { get; set; }
        public virtual ICollection<QuestComponents> QuestComponents { get; set; }
        public virtual ICollection<BuildPlanComponent> BuildPlanComponents { get; set; }
        public virtual ICollection<BuildPlanResource> BuildPlanResources { get; set; }
        public virtual ICollection<BuildPlanResource> SourceBuildPlanResources { get; set; }


    }
}
