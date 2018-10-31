using System;

namespace NSCSC.Shared.Models.CRM.Customers.ProductManagement
{
    public  class HardwareServiceModels
    {
        public string AssetID { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public int AdditionType { get; set; }
        public DateTime? BoughtTime { get; set; }
        public string SerialNo { get; set; }
        public string ReceiptNo { get; set; }
        public int State { get; set; }
        /*Client*/
        public string sAdditionType { get; set; }
        public string Status { get; set; }
        public string sBoughtTime { get; set; }
    }
}
