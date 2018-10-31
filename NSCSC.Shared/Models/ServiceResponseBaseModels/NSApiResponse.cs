
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models
{
    public class NSApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Error { get; set; }
        public object Data { get; set; }
    }
}
