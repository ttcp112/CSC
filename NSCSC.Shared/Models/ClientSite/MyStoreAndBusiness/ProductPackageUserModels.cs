using NSCSC.Shared.Models.ClientSite.MyProfile;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
{
    public class ProductPackageUserModels
    {
        public string AssetID { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public int ProductType { get; set; }
        public string Image { get; set; }
        public string PeriodName { get; set; }
        public int PeriodType { get; set; }
        public int Period { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string sExpiryDate { get; set; }
        public int? NumberOfUnit { get; set; }
        public int? NumberOfAccount { get; set; }
        public bool IsActive { get; set; }
        public bool IsUnlimitedUnit { get; set; }
        public string sIsUnlimitedUnit { get; set; }
        public bool IsUnlimitedAccount { get; set; }
        public string sIsUnlimitedAccount { get; set; }
        public bool IsAllFunction { get; set; }
        public string sFunctions { get; set; }
        public DateTime? ActiveTime { get; set; }
        public List<ProductUserModels> ListProduct { get; set; }
        public List<FunctionUserModels> ListFunction { get; set; }
        public List<AdditionUserModels> ListComposite { get; set; }
        public List<AdditionUserModels> ListAddition { get; set; }
        public List<DeviceUserModels> ListDevice { get; set; }
        public List<ProductPriceModels> ListPrices { get; set; }
        public List<SelectListItem> ListItemProduct { get; set; }
        public List<ApplyStore> ListStoreApply { get; set; }
        public ProductCategory Category { get; set; }
        public string ProductID { get; set; }
        public bool IsProduct { get; set; }
        public bool ApplyExtend { get; set; } // Check item can extend
        //Client
        public int TabId { get; set; }
        public AssignToStore AssignStore { get; set; }
        public bool IsFree { get; set; }
        public string ProductAppliOnExtendID { get; set; }
        public ProductPackageUserModels()
        {
            ListProduct = new List<ProductUserModels>();
            ListFunction = new List<FunctionUserModels>();
            ListComposite = new List<AdditionUserModels>();
            ListAddition = new List<AdditionUserModels>();
            ListDevice = new List<DeviceUserModels>();
            ListPrices = new List<ProductPriceModels>();
            ListItemProduct = new List<SelectListItem>();
        }
    }
    public class AssignToStore
    {
        public string tempAssetID { get; set; }
        public List<AssignStore> ListAssignStore { get; set; }
        public AssignToStore()
        {
            ListAssignStore = new List<AssignStore>();
        }
    }
    public class AssignStore
    {
        public int Offset { get; set; }
        public string StoreName { get; set; }
        public string StoreID { get; set; }
        public bool IsApply { get; set; }
        public int StoreType { get; set; }
        public string Industry { get; set; }
        public string Address { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime? ActivatedDate { get; set; }
    }
    public class ProductCategory
    {
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int Type { get; set; }
        public bool IsActive { get; set; }
        public string ProductID { get; set; }
        public bool IsFreeTrial { get; set; }
    }
}
