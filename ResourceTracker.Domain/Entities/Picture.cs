using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Domain.Entities
{
    public class Picture
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public string AltText { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UploadedBy { get; set; }
        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<Quest> Quests { get; set; }
        public virtual ICollection<Component> Components { get; set; }
    }
}
