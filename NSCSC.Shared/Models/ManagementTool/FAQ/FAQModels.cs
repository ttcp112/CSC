using NSCSC.Shared.Models.ManagementTool.Topics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.ManagementTool.FAQ
{    
    public class FAQModels
    {
        public string ID { get; set; }
        public string TopicID { get; set; }
        public string TopicName { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool IsActive { get; set; }
    }

    public class FAQViewModels
    {
        public string IDTopic { get; set; }
        public List<SelectListItem> ListTopic { get; set; }
        public List<TopicModel> ListTopicItem { get; set; }
        public List<FAQModels> ListIFAQtem { get; set; }
        public FAQViewModels()
        {
            ListTopic = new List<SelectListItem>();
        }
    }
}
