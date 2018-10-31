using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.SandBox.Inventory.Discount
{
    public class DiscountModels
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int ValueType { get; set; }
        public double Value { get; set; }
        public DateTime? ApplyFrom { get; set; }
        public DateTime? ApplyTo { get; set; }
    }

    public class DiscountViewModels
    {
        public string SearchString { get; set; }
        public List<DiscountModels> ListItem { get; set; }

        public DiscountViewModels()
        {
            ListItem = new List<DiscountModels>();
        }
    }
}
