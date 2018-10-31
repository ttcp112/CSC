using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.SandBox.Inventory.Product
{
    public class ProductCategoryModels
    {
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int Type { get; set; }
        public bool IsActive { get; set; }
        /*Client*/
        public int OffSet { get; set; }
        public int Status { get; set; }
        public bool IsFreeTrial { get; set; }
    }
}
