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
        public string Location { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public int? PictureId { get; set; }
        public virtual User User { get; set; }
        public virtual Game Game { get; set; }
        public virtual Picture Picture { get; set; }
        public virtual ICollection<QuestComponents> QuestComponents { get; set; }
    }
}
