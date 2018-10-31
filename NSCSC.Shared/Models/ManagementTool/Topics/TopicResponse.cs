using NSCSC.Shared.Models.ManagementTool.FAQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ManagementTool.Topics
{
    public class TopicResponse
    {
        public class ResponseGetTopic : NSApiResponseBase
        {
            public List<TopicDTO> ListTopic { get; set; }
        }
    }
}
