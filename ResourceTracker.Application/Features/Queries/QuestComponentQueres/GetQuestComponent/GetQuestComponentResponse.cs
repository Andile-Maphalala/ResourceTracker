using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Application.Features.Queries.QuestComponentQueres.GetQuestComponent
{
    public class GetQuestComponentResponse
    {
        public int Id { get; set; }

        public int AmountRequired { get; set; }

        public int AmountAquired { get; set; }

        public string ComponentName { get; set; }

        public int ComponentType { get; set; }

        public string ComponentTypeName { get; set; }

        public int QuestId { get; set; }
        public string QuestName { get; set; }

    }
}
