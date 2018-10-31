using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.SandBox.Inventory.Discount
{
    public class DiscountCodeModels
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public int State { get; set; } 
        public DateTime ExpiredDate { get; set; }
        /*Client*/
        public int OffSet { get; set; }
        public int Status { get; set; }
        public string Type { get; set; } /*Active|Used|Block*/
    }
}
