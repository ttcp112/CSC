using System;
using System.Collections.Generic;
using NSCSC.Shared.Models.SandBox.Inventory.Product;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
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
        public List<ProductCategoryModels> ListCategory { get; set; } // Updated 12062017
        public List<ProductApplyStoreModels> ListProduct { get; set; }
        //==========Client
        public string sRemainingLocation { get; set; }
        public string sExpiredTime { get; set; }
        public string sPeriod { get; set; }
        public AdditionApply AdditionApply { get; set; }
        public bool IsAppliedStore { get; set; }
        public DateTime? ActiveTime { get; set; }
    }

    public class AdditionApply
    {
        public string ID { get; set; } //AssetID
        public string ProductID { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }//ProductName
        public string ParentID { get; set; }//parent assetID
        public string ProductParentID { get; set; } //parent product
        public string ParentName { get; set; }
    }
}
