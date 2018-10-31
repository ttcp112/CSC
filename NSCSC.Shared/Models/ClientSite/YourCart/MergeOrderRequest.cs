using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ClientSite.YourCart
{
    public class MergeOrderRequest
    {
        public string OrderIDFrom { get; set; }
        public string OrderIDTo { get; set; }
    }
}
