using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.CRM.Customers
{
    public class ApplyProductOnStoreModels
    {
        public string OrderID { get; set; }
        public string OrderNo { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public int Period { get; set; }
        public int PeriodType { get; set; }
    }
}
