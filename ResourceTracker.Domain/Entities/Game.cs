using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Domain.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] CoverImage { get; set; }
        public virtual ICollection<Quest> Quests { get; set; }
        public virtual ICollection<Component> Components { get; set; }
        public virtual ICollection<BuildPlan> BuildPlans { get; set; }
    }
}
