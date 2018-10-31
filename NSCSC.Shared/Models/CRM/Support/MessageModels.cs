using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.CRM.Support
{
    public class MessageModels
    {
        public string ID { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Account { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
    }
}
