using System;

namespace NSCSC.Shared.Models.CRM.Customers.ProductManagement
{
    public class AccountManagementModels
    {
        public string AssetAccountID { get; set; }
        public string AssetID { get; set; }
        public string Account { get; set; }
        public string LicenseKey { get; set; }
        public DateTime? ActivateTime { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int ProductType { get; set; }
        public int PeriodType { get; set; }
        /*Client*/
        public string sType { get; set; }
        public string Status { get; set; }
        public string sActivateTime { get; set; }
        public string sExpiryDate { get; set; }
    }
}
