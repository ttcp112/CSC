using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Payments.Paypal.DTO
{
    public class NSPaymentExecuteRequest
    {
        public string PayerId { get; set; }

        public string PaymentId { get; set; }
    }
}
