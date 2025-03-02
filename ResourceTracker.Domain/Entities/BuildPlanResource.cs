using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Domain.Entities
{
    public class BuildPlanResource
    {
        public int Id { get; set; }

        public int TotalQuantityNeeded { get; set; }

        public int QuantityGathered { get; set; }

        public int ComponentId { get; set; }

        public int SourceComponentId { get; set; }

        public int BuildPlanId { get; set; }

        public virtual Component Component { get; set; }

        public virtual Component SourceComponent { get; set; }

        public virtual BuildPlan BuildPlan { get; set; }
    }
}
