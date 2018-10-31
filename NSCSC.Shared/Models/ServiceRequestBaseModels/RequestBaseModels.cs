using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models
{
    public class RequestBaseModels
    {
        public string CreatedUser { get; set; }
        public string ID { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string SearchString { get; set; }
        public string ReasonDelete { get; set; }

        public RequestBaseModels()
        {
            PageSize = 6000;
            PageIndex = 1;
        }
    }
}
