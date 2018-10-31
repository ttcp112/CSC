using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.CRM.Customers
{

    public class ProductPackageAdminModels
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
        public string Image { get; set; }
        public int Period { get; set; }
        public int NumberOfStore { get; set; }
        public bool IsUnlimitedStore { get; set; }
        public int NumberOfUnit { get; set; }
        public bool IsUnlimitedUnit { get; set; }
        public int NumberOfAccount { get; set; }
        public bool IsUnlimitedAccount { get; set; }
        public bool IsAllFunction { get; set; }
        public List<ProductAdminModels> ListProduct { get; set; }
        public List<FunctionAdminModels> ListFunction { get; set; }
        public List<AdditionAdminModels> ListComposite { get; set; }
        public List<AdditionAdminModels> ListAddition { get; set; }
        public List<DeviceAdminModels> ListDevice { get; set; }
        public List<ProductPriceAdminModels> ListPrices { get; set; }
        public List<SelectListItem> ListItemProduct { get; set; }
        public string CustomerID { get; set; }
        public string ProductAppliOn { get; set; }
        public string fPeriod { get; set; }
        public List<ApplyStore> ListStoreApply { get; set; }
        public ProductCategory Category { get; set; }
        public AssignToStore AssignStore { get; set; }

        public ProductPackageAdminModels()
        {
            ListProduct = new List<ProductAdminModels>();
            ListFunction = new List<FunctionAdminModels>();
            ListComposite = new List<AdditionAdminModels>();
            ListAddition = new List<AdditionAdminModels>();
            ListDevice = new List<DeviceAdminModels>();
            ListPrices = new List<ProductPriceAdminModels>();
            ListItemProduct = new List<SelectListItem>();
            ListStoreApply = new List<ApplyStore>();
           
        }
    }
    public class AssignToStore {
        public string AssetID { get; set; }
        public List<AssignStore> ListAssignStore { get; set; }
        public AssignToStore() {
            ListAssignStore = new List<AssignStore>();
        }
    }
    public class ProductAdminModels
    {
        public string AssetID { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public int ProductType { get; set; }
        public string Image { get; set; }
        public int PeriodType { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int NumberOfUnit { get; set; }
        public bool IsUnlimitedUnit { get; set; }
        public int NumberOfAccount { get; set; }
        public bool IsUnlimitedAccount { get; set; }
        public int NumberOfStore { get; set; }
        public bool IsUnlimitedStore { get; set; }
        public bool IsActive { get; set; }
        public string CustomerID { get; set; }
        public List<FunctionAdminModels> ListFunction { get; set; }
        //public List<AdditionAdminModels> ListFunction { get; set; }
        public List<ApplyStore> ListStoreApply { get; set; }
        public ProductCategory Category { get; set; }
        public AssignToStore AssignStore { get; set; }


        public ProductAdminModels()
        {
            ListFunction = new List<FunctionAdminModels>();
            ListStoreApply = new List<ApplyStore>();
        }
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
    public class ApplyStore
    {
        public string ID { get; set; }
        public string StoreID { get; set; }
        public string StoreName { get; set; }
        public bool IsExtend { get; set; }
        public bool IsDelete { get; set; }
        public string ProductID { get; set; }
    }
    public class AssignStore {
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
    public class AdditionAdminModels
    {
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string SerialNo { get; set; }
        public int Type { get; set; }
        public int State { get; set; }
        public string fType { get; set; }
        public string fState { get; set; }
        public int ProductType { get; set; }
    }

    public class DeviceAdminModels
    {
        public string ID { get; set; }
        public string UUID { get; set; }
        public DateTime? ActiveTime { get; set; }
        public string LicenseKey { get; set; }
        public int ProductType { get; set; }
        public bool IsActive { get; set; }
        public string ProductID { get; set; }
    }

    public class FunctionAdminModels
    {
        public string FunctionID { get; set; }
        public string FunctionName { get; set; }
    }

    public class ProductPriceAdminModels
    {
        public string ID { get; set; }
        public double Price { get; set; }
        public double Period { get; set; }
        public int PeriodType { get; set; }
        public bool IsExtend { get; set; }
        public string PeriodName { get; set; }

        public string CurrencySymbol { get; set; }

    }
}
