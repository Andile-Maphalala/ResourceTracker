using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Domain.Entities
{
    public class QuestComponents
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public int ComponentId { get; set; }

        public int QuestId { get; set; }

        public virtual Component Component { get; set; }

        public virtual Quest Quest { get; set; }

    }
}
