using NSCSC.Shared.Models.ClientSite.MyProfile;
using System.Collections.Generic;

namespace NSCSC.Shared.Models.ClientSite.YourCart
{
    public class YourCartRequest
    {
        public string CusID { get; set; }
        public string OrderID { get; set; }
        public List<Item> ListItems { get; set; }
        public string Note { get; set; }
        /* for [productID] when  apply discount of item*/
        public string ID { get; set; }
        public string DiscountCode { get; set; }
        public string CreatedUser { get; set; }
        public string OrderDetailID { get; set; }
        //=============Pay
        public string PaymentMethodID { get; set; }
        public double AmountPay { get; set; }
        public bool IsManual { get; set; }
        public bool IsZero { get; set; }
        public bool? isCheckvalid { get; set; }
        public string TransactionID { get; set; }

       // public bool isFree { get; set; }
        public bool IsFree { get; set; }
        public YourCartRequest()
        {
            ListItems = new List<Item>();
            isCheckvalid = null;
        }
    }
}
