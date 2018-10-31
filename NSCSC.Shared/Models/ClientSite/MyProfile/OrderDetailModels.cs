using NSCSC.Shared.Models.CRM.Customers;
using NSCSC.Shared.Models.SandBox.Inventory.Product;
using NSCSC.Shared.Models.Settings.PaymentMethods;
using NSCSC.Shared.Models.Settings.Tax;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.ClientSite.MyProfile
{
    public class OrderDetailModels
    {
        public string ID { get; set; }
        public string OrderNo { get; set; }
        public string ReceiptNo { get; set; }
        public double Total { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime PaidTime { get; set; }
        public List<PaymentDetail> PaidByMethod { get; set; }
        public string sPaidByMethod { get; set; }

        public string Description { get; set; }
        //public List<Item> ListItem { get; set; }
        public BillDiscount BillDiscount { get; set; }

        /*update*/
        public double Tax { get; set; }
        public double TotalPromotion { get; set; }
        public double SubTotal { get; set; }
        public List<Item> ListItems { get; set; }
        public TaxModels TaxInfo { get; set; }
        public double TotalDiscount { get; set; }
        public int DiscountType { get; set; }
        public double DiscountOnTotalCart { get; set; }


        public Item DiscountTotalBill { get; set; }

        public string sTaxName { set; get; }
        public double sTaxValue { set; get; }
        public string CusId { get; set; }
        public string CustomerID { get; set; }
        public int TotalQuantity { get; set; }

        //====currency
        public string CurrencySymbol { get; set; }
        public Customermodel CustomerDetail { get; set; }
        public MerchantDTO MerchantDetail { get; set; }
        public double TotalBill { get; set; }
        public List<PaymentMethodModels> LstPayment { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public string ImageURL { get; set; }
        public bool IsShowButtonRefund { get; set; }
        public bool IsPad { get; set; }
       // public bool isFree { get; set; }
        public bool IsFree { get; set; }
        public bool HasMerchant { get; set; }
        public bool IsReseller { get; set; }
        public OrderDetailModels()
        {
            //ListItem = new List<Item>();
            /*update*/
            ListItems = new List<Item>();
            PaidByMethod = new List<PaymentDetail>();
            TaxInfo = new TaxModels();

            DiscountTotalBill = new Item();
            CustomerDetail = new Customermodel();
            LstPayment = new List<PaymentMethodModels>();
            MerchantDetail = new MerchantDTO();
        }
    }

    public class BillDiscount
    {
        public string ID { get; set; }
        public string DiscountID { get; set; }
        public string DiscountName { get; set; }
        public string DiscountCode { get; set; }
        public double DiscountAmount { get; set; }
        public double DiscountValue { get; set; }
        public int DiscountType { get; set; }
    }

    public class Item
    {
        public string ID { get; set; }

        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductType { get; set; }
        public string ParentAddition { get; set; }
        public int AdditionType { get; set; }

        public string ImageURL { get; set; }
        public int Period { get; set; }
        public int PeriodType { get; set; }

        public double Quantity { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
        public double SubTotal { get; set; }
        public string PromotionID { get; set; }
        public string PromotionName { get; set; }
        public double PromotionAmount { get; set; }
        public bool IsApplyPromotion { get; set; }

        public string DiscountID { get; set; }
        public string DiscountName { get; set; }
        public double DiscountAmount { get; set; }
        public double DiscountValue { get; set; }
        public int DiscountType { get; set; }
        public string DiscountTypeName { get; set; }
        public string DiscountDisplay { get; set; }

        public string Description { get; set; }
        public int ItemGroup { get; set; }
        public List<Item> ListItem { get; set; }
        public List<ApplyStore> ListStoreApply { get; set; }
        //check out
        public string sStoreCompleted { get; set; }
        /*Update*/
        public int NumOfStore { get; set; }
        public string sNumOfStore { get; set; }
        public bool IsDelete { get; set; }
        public List<ItemFunction> ListFunction { get; set; }
        public List<AdditionApply> ListAdditionApply { get; set; }
        // => if -1 -> Unlimited
        public int NumOfAccount { get; set; }
        public bool IsFullFunction { get; set; }
        public int AmountOfUnit { get; set; }

        //update 09-10-2017 - Vien
        public List<ProductCompositeModels> ListComposite { get; set; }

        /*Client*/
        public string PeriodName { get; set; }
        public string TaxName { get; set; }
        public string sNumOfAccount { get; set; }
        public string sFunction { get; set; }
        public string sAmountOfUnit { get; set; }

        //public List<Item> ListComposite { get; set; }
        public List<Item> ListAdditional { get; set; }
        public List<ProductCompositeModels> ListProductOfPackage { get; set; }

        public string AppliedOn { get; set; }
        public string ApplicableOn { get; set; }

        public int OffSet { get; set; }

        public string CurrencySymbol { get; set; }
        public bool IsShowCheckRefurn { get; set; }
        public bool IsExtend { get; set; }
        public bool IsRefund { get; set; }
        public List<ProductCategoryModels> ListCategory { get; set; } // Updated 12082017
        public int Type { get; set; }
        /* update 06/09/2018 */
        public string NamePeriodType { get; set; }
        public string PeriodTime { get; set; }
        public string CusID { get; set; }
        public string sPeriodType { get; set; }
        public List<SelectListItem> ListPeriodType { get; set; }
        public List<ProductPriceModels> ListPrice { get; set; }
        public Item()
        {
            ListItem = new List<Item>();
            ListStoreApply = new List<ApplyStore>();

            ListFunction = new List<ItemFunction>();
            ListAdditionApply = new List<AdditionApply>();

            ListComposite = new List<ProductCompositeModels>();
            ListAdditional = new List<Item>();

            ListProductOfPackage = new List<ProductCompositeModels>();
            ListCategory = new List<ProductCategoryModels>(); // Updated 12082017
            ListPeriodType = new List<SelectListItem>();
            ListPrice = new List<ProductPriceModels>();
        }
    }

    public class ApplyStore
    {
        public string ID { get; set; }
        public string StoreID { get; set; }
        public string StoreName { get; set; }
        public bool IsExtend { get; set; }
        public bool IsDelete { get; set; }
        //==========
        public string sStatus { get; set; }
        public string sTitle { get; set; }
        public string ProductID { get; set; }
    }

    public class ItemFunction
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    public class AdditionApply
    {
        public string ID { get; set; }              //AssetID
        public string ProductID { get; set; }
        public string Name { get; set; }            //ProductName
        public string ParentID { get; set; }        //parent assetID
        public string ProductParentID { get; set; } //parent product
        public string ParentName { get; set; }
        public int Type { get; set; }
    }

    public class PaymentDetail
    {
        public string Name { get; set; }
        public double Amount { get; set; }
        public string CurrencySymbol { get; set; }
    }

    public class HistoriesPaid
    {
        public double Change { get; set; }
        public double Remaining { get; set; }
        public double TotalAmount { get; set; }
        public double Received { get; set; }
        public string CurrencySymbol { get; set; }
        public List<PaymentDetail> ListPaidMethod { get; set; }
        public bool IsPaid { get; set; }

        public HistoriesPaid()
        {
            ListPaidMethod = new List<PaymentDetail>();
        }
    }

    public class DonePayCheckOut
    {
        public DateTime? ExpiryDate { get; set; }
        public double Period { get; set; }
        public int PeriodType { get; set; }
        public string PeriodName { get; set; }
        public string ProductAppliedName { get; set; }
    }
   
}

