using System;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
{
    public class AdditionFunctionModels
    {
        public string AssetFunctionID { get; set; }
        public string FunctionID { get; set; }
        public string FunctionName { get; set; }
        public int Type { get; set; }
        public string LicenseKey { get; set; }
        public string ParentLicenseKey { get; set; }
        public DateTime? ActivateTime { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlock { get; set; }
        public int PeriodType { get; set; }
        /*Client*/
        public string Status { get; set; }
        public string sExpiryDate { get; set; }
        public string sActivateTime { get; set; }
    }
}
