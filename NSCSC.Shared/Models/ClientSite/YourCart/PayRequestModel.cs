using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ClientSite.YourCart
{
    public class PayRequestModel: RequestBaseModels
    {
        public string OrderID { get; set; }
        public string PaymentMethodID { get; set; }
        public double AmountPay { get; set; }
    }
}
