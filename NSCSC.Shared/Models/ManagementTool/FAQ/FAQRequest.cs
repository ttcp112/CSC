using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ManagementTool.FAQ
{
    public class CreateOrEditFAQRequest : RequestBaseModels
    {
        public FAQDTO FAQ { get; set; }
    }
    public class DeleteFAQRequest : RequestBaseModels { }
    public class GetFAQRequest : RequestBaseModels  { }

    public class FAQDTO
    {
        public string ID { get; set; }
        public string TopicID { get; set; }
        public string TopicName { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool IsActive { get; set; }
    }

}
