using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.CRM.Customers
{
    public class ItemRefundModels
    {
        public string ProductName { get; set; }
        public int ProductType { get; set; }
        public int AdditionType { get; set; }
        public string sProductType { get; set; }
        public string sType { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalRefund { get; set; }
        public string CurrencySymbol { get; set; }
        public string RefundReason { get; set; }
        public int PeriodType { get; set; }
        public int Period { get; set; }
        public string OrderId { get; set; }
        public string OrderDetailId { get; set; }
    }

    public class DoneRefundModels
    {
        public string OrderID { get; set; }
        public float TotalAmount { get; set; }
        public string Description { get; set; }
        public List<RefundDetailModels> ListDetail { get; set; }
    }

    public class RefundDetailModels
    {
        public string OrderDetailID { get; set; }
        public float Amount { get; set; }
    }
}
