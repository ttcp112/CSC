using NSCSC.Shared.Models.ClientSite.MyProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
{
    public class ProductUserModels
    {
        public string AssetID { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public int ProductType { get; set; }
        public string Image { get; set; }
        public int PeriodType { get; set; }
        public string sExpiryDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ActiveTime { get; set; }
        public int? NumberOfUnit { get; set; }
        public int? NumberOfAccount { get; set; }
        public bool IsActive { get; set; }
        public bool IsUnlimitedUnit { get; set; }
        public bool IsUnlimitedAccount { get; set; }
        
        public List<DeviceUserModels> ListDevices { get; set; }
        public List<FunctionUserModels> ListFunction { get; set; }
        public List<ApplyStore> ListStoreApply { get; set; }
        public ProductCategory Category { get; set; }
        public AssignToStore AssignStore { get; set; }

        public ProductUserModels()
        {
            ListDevices = new List<DeviceUserModels>();
            ListFunction = new List<FunctionUserModels>();
            ListStoreApply = new List<ApplyStore>();
        }
    }
}
