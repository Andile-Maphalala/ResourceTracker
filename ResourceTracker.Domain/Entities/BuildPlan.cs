using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Domain.Entities
{
    public class BuildPlan
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int UserId { get; set; }

        public int GameId { get; set; }

        public virtual User User { get; set; }
        public virtual Game Game { get; set; }

        public virtual ICollection<BuildPlanComponent> BuildPlanComponents { get; set; }
        public virtual ICollection<BuildPlanResource> BuildPlanResources { get; set; }

    }
}
