using NSCSC.Shared.Models.OurProducts;
using NSCSC.Shared.Models.OurProducts.Product;
using NSCSC.Shared.Models.Sandbox.Inventory.Product;
using NSCSC.Shared.Models.SandBox.Inventory.Product;
using System;
using System.Collections.Generic;
using System.Web.Mvc;


namespace NSCSC.Shared.Models.ClientSite.OurProDucts
{
    public class OurProDuctsModel
    {
        public int Index { get; set; }
        public string ID { get; set; }
        public int Type { get; set; }
        public int ProductType { get; set; }
        public int AdditionType { get; set; }
        public int Sequence { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string Code { get; set; }
        public int AmountOfUnit { get; set; }
        public bool IsUnlimitedAmountOfUnit { get; set; }
        public string sUnlimitedAmountOfUnit { get; set; }
        public DateTime SaleFrom { get; set; }
        public DateTime SaleTo { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublic { get; set; }
        public bool IsExtend { get; set; }
        public int IncludeStore { get; set; }
        public bool IsUnlimitedIncludeStore { get; set; }
        public int NumberOfAccount { get; set; }
        public bool IsUnlimitedNumberOfAccount { get; set; }
        public string sUnlimitedNumberOfAccount { get; set; }

        public bool IsDisplayWeb { get; set; }
        public bool IsIntegrate { get; set; }
        public bool IsIncludeLocalServer { get; set; }
        public bool IsIncludeCloudServer { get; set; }
        public string Description { get; set; }
        public List<OurProDuctsModel> ListProducts { get; set; }
        public double Price { get; set; }
        public List<ProductCompositeModels> ListComposite { get; set; }
        public List<ProductCompositeModels> ListProductOnPackage { get; set; }
        public List<ProductPriceModels> ListPrice { get; set; }
        public List<ProductFunctionModels> ListFunction { get; set; }
        public bool IsFullFunction { get; set; }
        public string sFullFunction { get; set; }
        public bool IsFullCate { get; set; }

        /*trongntn*/
        public List<ProductCategoryModels> ListCategory { get; set; }
        public List<OurProDuctsModel> ListProductChild { get; set; }
        /* when product is package, it contain list product */

        //for Addition

        public int MinDay { get; set; }
        public int MaxDay { get; set; }

        public string CusId { get; set; }

        public string ApplicableProducts { get; set; }
        public double TotalPrice { get; set; }
        public string PeriodTime { get; set; }
        public double Quantity { get; set; }
        public string OrderID { get; set; }

        public int NumOfStore { get; set; }
        public string PromotionID { get; set; }
        public string PromotionName { get; set; }
        public double PromotionAmount { get; set; }
        public bool IsApplyPromotion { get; set; }
        public string DiscountID { get; set; }
        public string DiscountName { get; set; }
        public double DiscountAmount { get; set; }
        public double DiscountValue { get; set; }
        public int DiscountType { get; set; }
        public int ItemGroup { get; set; }
        public bool IsDelete { get; set; }
        public int Period { get; set; }
        public int PeriodType { get; set; }
        public string sPeriodType { get; set; }
        public double AdditionPrice { get; set; }

        public List<SelectListItem> ListProductApplicable { get; set; } // For popup choose product to apply addition in package
        public string ApplyProductId { get; set; } // For add addition to product in package

        public List<BuyingAdditionModels> ListBuyingAddition { get; set; }
        public List<BuyingAdditionViewModels> ListBuyAdditionPopup { get; set; }
        /*Client*/
        public int Status { get; set; }
        public string AdditionTypeName { get; set; }

        // For Add/Edit Additions in YourCart
        public string ItemID { get; set; }

        public string CurrencySymbol { get; set; }
        public string CategoryId { get; set; }
        public double Amount { get; set; } // For quantity * price
        //update 07/09/2018
        public string sAdditionType { get; set; }
        public bool Selected { get; set; }
        public OurProDuctsModel()
        {
            ListBuyingAddition = new List<BuyingAdditionModels>();
            ListProductChild = new List<OurProDuctsModel>();
            ListBuyAdditionPopup = new List<BuyingAdditionViewModels>();
        }
    }

    public class CategoryDTO
    {
        public string ID { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public int NumberOfProduct { get; set; }
        public bool IsActive { get; set; }
        public string ImageUrl { get; set; }
    }

    public class ProductExtendModel
    {
        public string AssetID { get; set; }
        public int Period { get; set; }
        public int PeriodType { get; set; }
        public double Price { get; set; }
    }
}
