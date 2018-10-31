using NSCSC.Payments.Paypal.Helpers;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Payments.Paypal.DTO
{
    public class NSPaymentCreateRequest
    {
        public Payment Payment { get; set; }

        public class Builder
        {
            private List<NSItem> nsitems = new List<NSItem>();

            private double tax = 0;

            private double shipping = 0;

            private string currency = "SGD";

            private string baseURI;

            public NSPaymentCreateRequest Build()
            {
                double subtotal = 0;

                // ###Items
                // Items within a transaction.
                var itemList = new ItemList
                {
                    items = new List<Item>()
                };
                foreach (var nsitem in nsitems)
                {
                    var item = new Item
                    {
                        name = nsitem.Name,
                        currency = currency,
                        price = nsitem.Price.ToString(),
                        quantity = nsitem.Quantity.ToString(),
                        sku = nsitem.SKU
                    };
                    itemList.items.Add(item);

                    subtotal += nsitem.Price * nsitem.Quantity;
                }

                // ###Payer
                // A resource representing a Payer that funds a payment
                // Payment Method
                // as `paypal`
                var payer = new Payer() { payment_method = "paypal" };

                // ###Redirect URLS
                // These URLs will determine how the user is redirected from PayPal once they have either approved or canceled the payment.
                var guid = Convert.ToString((new Random()).Next(100000));
                var redirectUrl = baseURI + "guid=" + guid;
                var redirUrls = new RedirectUrls()
                {
                    cancel_url = redirectUrl + "&cancel=true",
                    return_url = redirectUrl
                };

                // ###Details
                // Let's you specify details of a payment amount.
                var details = new Details()
                {
                    tax = tax.ToString(),
                    shipping = shipping.ToString(),
                    subtotal = subtotal.ToString()
                };

                // ###Amount
                // Let's you specify a payment amount.
                var amount = new Amount()
                {
                    currency = currency,
                    total = (tax + shipping + subtotal).ToString(), // Total must be equal to sum of shipping, tax and subtotal.
                    details = details
                };

                // ###Transaction
                // A transaction defines the contract of a
                // payment - what is the payment for and who
                // is fulfilling it. 
                var transactionList = new List<Transaction>();

                // The Payment creation API requires a list of
                // Transaction; add the created `Transaction`
                // to a List
                transactionList.Add(new Transaction()
                {
                    description = "Transaction description.",
                    invoice_number = Common.GetRandomInvoiceNumber(),
                    amount = amount,
                    item_list = itemList
                });

                // ###Payment
                // A Payment Resource; create one using
                // the above types and intent as `sale` or `authorize`
                var payment = new Payment()
                {
                    intent = "sale",
                    payer = payer,
                    transactions = transactionList,
                    redirect_urls = redirUrls
                };

                return new NSPaymentCreateRequest { Payment = payment };
            }

            #region Setters

            public Builder AddListItem(List<NSItem> item)
            {
                foreach (var nsitem in item)
                {
                    nsitems.Add(nsitem);
                }
                return this;
            }

            public Builder AddItem(NSItem item)
            {
                nsitems.Add(item);
                return this;
            }

            public Builder SetTax(double tax)
            {
                this.tax = tax;
                return this;
            }

            public Builder SetShipping(double shipping)
            {
                this.shipping = shipping;
                return this;
            }

            public Builder SetCurrency(string currency)
            {
                this.currency = currency;
                return this;
            }

            public Builder SetBaseURI(string baseURI)
            {
                this.baseURI = baseURI;
                return this;
            }
        }

        #endregion

    }
}
