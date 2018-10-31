using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.SandBox.Inventory.Discount
{
    public class DiscountCategoryModels
    {
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int CateType { get; set; }
        /*Client*/
        public int OffSet { get; set; }
        public string Type { get; set; } /*Package|Additions|Category*/
        public int Status { get; set; }
    }
}
