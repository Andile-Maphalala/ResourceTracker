using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Domain.Entities
{
    public class Quest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public byte[] Image { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<QuestComponents> QuestComponents { get; set; }
        public virtual ICollection<ResourceCollection> ResourceCollections { get; set; }
    }
}
