using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ManagementTool.Topics
{
    public class CreateOrEditTopicRequest : RequestBaseModels
    {
        public TopicDTO TopicDetail { get; set; }
    }
    public class DeleteTopicRequest : RequestBaseModels { }
    public class GetTopicRequest : RequestBaseModels { }
    public class TopicDTO
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
