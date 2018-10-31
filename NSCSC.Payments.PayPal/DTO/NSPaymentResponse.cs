using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Payments.Paypal.DTO
{
    public class NSPaymentResponse
    {
        public class NSCreation
        {
            public string PaymentId { get; set; }

            public string Url { get; set; }
        }

        public class NSExecution
        {
            public string State { get; set; }
            public string TransactionID { get; set; }
            public string Note { get; set; }
        }
    }
}
