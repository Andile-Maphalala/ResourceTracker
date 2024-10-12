using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Domain.Entities
{
    public class ResourceCollection
    {
        public int Id { get; set; }

        public int ItemsGathered { get; set; }

        public int QuestId { get; set; }

        public int ResourceId { get; set; }

        public virtual Quest Quest { get; set; }

        public virtual Resource Resource { get; set; }
    }
}
