using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ClientSite.YourCart
{
    public class DiscountModels
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int ValueType { get; set; }
        public double Value { get; set; }
    }
}
