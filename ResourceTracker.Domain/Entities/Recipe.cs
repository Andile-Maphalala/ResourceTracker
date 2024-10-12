using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Domain.Entities
{
    public class Recipe
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public int ParentComponentId { get; set; }

        public int? ResourceId { get; set; }

        public int? ComponentId { get; set; }

        public virtual Component ParentComponent { get; set; }

        public virtual Component Component { get; set; }

        public virtual Resource Resource { get; set; }
    }
}
