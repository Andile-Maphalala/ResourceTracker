using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Domain.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public int AmountMade { get; set; }
        public int ComponentId { get; set; }
        public int? RequiredFacilityId { get; set; }
        public virtual Component Component { get; set; }
        public virtual Component RequiredFacility { get; set; }
        public virtual Collection<RecipeComponent> RecipeComponents { get; set; }
    }
}
