using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models
{
    public class NSApiResponseBase : BaseModel
    {
        public string Description { get; set; }
        public string ID { get; set; }
        public string ReasonDelete { get; set; }
    }
}
