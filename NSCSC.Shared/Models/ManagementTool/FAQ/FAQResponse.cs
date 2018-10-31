using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ManagementTool.FAQ
{
    public class ResponseGetFAQ : NSApiResponseBase
    {
        public List<FAQDTO> ListFAQ { get; set; }

    }
}
