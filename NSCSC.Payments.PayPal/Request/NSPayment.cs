using NSCSC.Payments.Paypal.DTO;
using NSCSC.Payments.Paypal.Helpers;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Payments.Paypal.Request
{
    public class NSPayment
    {
        /// <summary>
        /// Creates and processes a payment. In the JSON request body, include a `payment`
        /// object with the intent, payer, and transactions. For PayPal payments, include
        /// redirect URLs in the `payment` object.
        /// </summary>
        public static NSPaymentResponse.NSCreation Create(NSPaymentCreateRequest request)
        {
            // Create a payment using a valid APIContext
            var createdPayment = request.Payment.Create(Configuration.GetAPIContext());

            // Using the `links` provided by the `createdPayment` object, we can give the user the option to redirect to PayPal to approve the payment.
            var links = createdPayment.links.GetEnumerator();
            while (links.MoveNext())
            {
                var link = links.Current;
                if (link.rel.ToLower().Trim().Equals("approval_url"))
                {
                    // Redirect to PayPal to approve the payment...", link.href
                    if (link.method.Equals("REDIRECT"))
                    {
                        return new NSPaymentResponse.NSCreation
                        {
                            PaymentId = createdPayment.id,
                            Url = link.href
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Executes, or completes, a PayPal payment that the payer has approved. You can
        /// optionally update selective payment information when you execute a payment.
        /// </summary>
        public static NSPaymentResponse.NSExecution Execute(NSPaymentExecuteRequest request)
        {
            // Using the information from the redirect, setup the payment to execute.
            var paymentExecution = new PaymentExecution() { payer_id = request.PayerId };
            var payment = new Payment() { id = request.PaymentId };
           
            // Execute the payment.
            var executedPayment = payment.Execute(Configuration.GetAPIContext(), paymentExecution);
            NSLog.Logger.Info("Payment PayPal Response : ", executedPayment);
            return new NSPaymentResponse.NSExecution { State = executedPayment.state, TransactionID = executedPayment.id, Note = executedPayment.transactions.Count > 0 ? executedPayment.transactions[0].invoice_number : "" };
        }
    }
}
