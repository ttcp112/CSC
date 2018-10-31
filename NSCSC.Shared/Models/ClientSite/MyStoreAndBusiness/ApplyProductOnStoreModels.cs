using System;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
{
    public class ApplyProductOnStoreModels
    {
        public string AssetID { get; set; }
        public string OrderID { get; set; }
        public string OrderNo { get; set; }
        public string ReceiptNo { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string ImageURL { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public int Period { get; set; }
        public int PeriodType { get; set; }
    }
}
