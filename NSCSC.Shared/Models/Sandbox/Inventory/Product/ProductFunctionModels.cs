using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.Sandbox.Inventory.Product
{
    public class ProductFunctionModels
    {
        public string FunctionID { get; set; }
        public string FunctionName { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDefault { get; set; }
    }
}
