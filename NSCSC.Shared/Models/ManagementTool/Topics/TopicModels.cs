using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ManagementTool.Topics
{
    public class TopicModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class TopicViewModels
    {
        public List<TopicModel> ListItem { get; set; }
    }
}
