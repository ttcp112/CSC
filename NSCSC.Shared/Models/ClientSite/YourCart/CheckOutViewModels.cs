using NSCSC.Shared.Models.ClientSite.MyProfile;
using NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness;
using NSCSC.Shared.Models.ManagementTool.Feedbacks;
using NSCSC.Shared.Models.Settings.PaymentMethods;
using System.Collections.Generic;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.ClientSite.YourCart
{
    public class CheckOutViewModels
    {
        public InputStoreViewModels InputStore { get; set; }
        public TermsAndConditionsViewModels TermsAndConditions { get; set; }
        public CheckOutView CheckOut { get; set; }
        public FeedbackDTO Feedback { get; set; }
        public List<Item> ListProduct { get; set; }
        public string CurrencySymbol { get; set; }

        public CheckOutViewModels()
        {
            ListProduct = new List<Item>();
            //============
            InputStore = new InputStoreViewModels();
            TermsAndConditions = new TermsAndConditionsViewModels();
            CheckOut = new CheckOutView();
            Feedback = new FeedbackDTO();
        }
    }

    public class InputStoreViewModels
    {
        public string ID { get; set; }
        public string OrderProductDetailID { get; set; }
        public string ProductName { get; set; }
        public int NumOfStore { get; set; }
        public string sNumOfStore { get; set; }
        public List<StoreDetailModels> ListStore { get; set; }
        public List<StoreModels> ListStoreSelect { get; set; }
        public string CusId { get; set; }
        public List<SelectListItem> ListTypePackage { get; set; }
        public int ProductType { get; set; }
        public int Type { get; set; }
        public InputStoreViewModels()
        {
            ListStore = new List<StoreDetailModels>();
            ListStoreSelect = new List<StoreModels>();
            ListTypePackage = new List<SelectListItem>();
        }
    }

    public class TermsAndConditionsViewModels
    {

    }

    public class CheckOutView
    {
        public string OrderID { get; set; }
        public string PaymentMethodID { get; set; }

        //Customer Information
        public string ImageURL { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Merchant { get; set; }
        public string Address { get; set; }

        //Order Summary
        public int NumberOfItems { get; set; }
        public string sNumberOfItems { get; set; }
        public double SubTotal { get; set; }
        public double Discount { get; set; }
        public double TotalPromotion { get; set; }
        public double Tax { get; set; }
        public string TaxName { get; set; }
        public double TaxValue { get; set; }

        public double TotalOrder { get; set; }
        public string Note { get; set; }
        //ListPayment
        public List<PaymentMethodModels> ListPayment { get; set; }

        public CheckOutView()
        {
            ListPayment = new List<PaymentMethodModels>();
        }
    }
}
