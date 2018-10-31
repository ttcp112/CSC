using System;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
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
        /*Client*/
        public string Status { get; set; }
        public string sExpiryDate { get; set; }
        public string sActivateTime { get; set; }
        public bool IsSupperAdmin { get; set; }
        public string sProductType { get; set; }
    }
}
