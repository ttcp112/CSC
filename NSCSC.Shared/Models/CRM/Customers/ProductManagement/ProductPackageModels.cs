using System;

namespace NSCSC.Shared.Models.CRM.Customers.ProductManagement
{
    public class ProductPackageModels
    {
        public string AssetID { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public int Type { get; set; }
        public DateTime? BoughtTime { get; set; }
        public DateTime? ActivateTime { get; set; }
        public int PeriodType { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ReceiptNo { get; set; }
        public bool IsActive { get; set; }
        /*Client*/
        public string sType { get; set; }
        public string Status { get; set; }
        public string sActivateTime { get; set; }
        public string sExpiryDate { get; set; }
    }
}
