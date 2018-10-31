using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.SandBox.Inventory.Discount
{
    public class DiscountRequest : RequestBaseModels
    {
        public DiscountDetailModels DiscountDetail { get; set; }
        public int NumberCode { get; set; }
        public List<string> ListID { get; set; }

        public DiscountRequest()
        {
            ListID = new List<string>();
        }
    }
}
