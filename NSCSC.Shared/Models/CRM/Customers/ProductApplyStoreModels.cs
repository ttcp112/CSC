using NSCSC.Shared.Models.ClientSite.MyProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.CRM.Customers
{
    public class ProductApplyStoreModels
    {
        public string ProductID { get; set; }
        public string AssetID { get; set; }
        public string ProductName { get; set; }
        public string ProductImageURL { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public int Period { get; set; }
        public int PeriodType { get; set; }
        public int RemainingLocation { get; set; }
        public bool IsUnlimitedLocation { get; set; }
        public int ProductType { get; set; }
        public int Type { get; set; } //Enum EType
        public string ParentID { get; set; }
        public List<ProductApplyStoreModels> ListProduct { get; set; }
        //==========Client
        public string sRemainingLocation { get; set; }
        public string sExpiredTime { get; set; }
        public string sPeriod { get; set; }
        public string CustomerID { get; set; }
        public string MerchantID { get; set; }
        public AdditionApply AdditionApply { get; set; }
        public DateTime? ActiveTime { get; set; }
        public bool IsAppliedStore { get; set; }
    }
    public class AdditionApply
    {
        public string ID { get; set; }
        public string ProductID { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }
}
