using NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness;
using System;
using System.Collections.Generic;

namespace NSCSC.Shared.Models.OurProducts.Addition
{
    public class ProductApplyAdditionModels
    {
        public string ProductID { get; set; }
        public int ProductType { get; set; }
        public string AssetID { get; set; }
        public string ParentID { get; set; } // Parent Asset Id
        public string ParentProductID { get; set; } // Parent Product Id
        public string ParentProductName { get; set; } // Parent Product Name
        public string ProductName { get; set; }
        public string ProductImageURL { get; set; }
        public DateTime BoughtTime { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public int Period { get; set; }
        public int PeriodType { get; set; }
        public List<ProductApplyAdditionModels> ListProduct { get; set; }
        public ProductPriceModels RecommendedPrice { get; set; }

        public string sExpiredTime { get; set; }
        public double Price { get; set; }
        public int OffSet { get; set; }
        public bool IsSelect { get; set; }

        public string Days { get; set; }
        public int MinDay { get; set; }
        public int MaxDay { get; set; }

        public string CurrencySymbol { get; set; }
        public ProductCategory Category { get; set; }
        public ProductApplyAdditionModels()
        {
            ListProduct = new List<ProductApplyAdditionModels>();
            RecommendedPrice = new ProductPriceModels();
        }
    }
}
