using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Domain.Entities
{
    public class Resource
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }
        public virtual ICollection<ResourceCollection> ResourceCollections { get; set; }
    }
}
