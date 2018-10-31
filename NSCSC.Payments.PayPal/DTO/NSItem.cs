using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Payments.Paypal.DTO
{
    public class NSItem
    {
        public string Name { get; set; }

        public string Currency { get; set; }

        public double Price { get; set; }

        public double Quantity { get; set; }

        public string SKU { get; set; }
    }
}
