using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.SandBox.Inventory.Product
{
    public class ProductPriceModels
    {
        public string ID { get; set; }
        public double Price { get; set; }
        public double Period { get; set; }
        public int PeriodType { get; set; }
        public bool IsActive { get; set; }
        public bool IsExtend { get; set; }
        /*Client*/
        public int OffSet { get; set; }
        public int Status { get; set; }
        public bool IsNew { get; set; }

        public int currentItemOffset { get; set; }
        public string NamePeriodType { get; set; }
        public int ProductOffSet { get; set; }
    }
}
