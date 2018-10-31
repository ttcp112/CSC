using NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness;
using NSCSC.Shared.Models.CRM.Customers;
using NSCSC.Shared.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace NSCSC.Shared.Models.ClientSite.MyProfile
{
    public class CustomerDetailModels
    {
        public Customermodel CustomerDetail { get; set; }
        public MerchantDTO MerchantDetail { get; set; }
        //public List<OrderModels> ListReceipt { get; set; }

        public List<PurchaseReceiptModels> ListReceipt { get; set; }

        public CustomerDetailModels()
        {
            CustomerDetail = new Customermodel();
            MerchantDetail = new MerchantDTO();
            ListReceipt = new List<PurchaseReceiptModels>();
            //ListReceipt = new List<OrderModels>();
        }
    }

    public class Customermodel
    {
        public string ID { get; set; }
        [Required]
        public string Name { get; set; }
        public string ImageURL { get; set; }
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Phone cannot be longer than 20 characters and less than 3 characters")]
        [RegularExpression(@"[0-9()+-]+", ErrorMessage = "Please only enter number, (, ), -, +")]
        [Required]
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public string NRIC { get; set; }
        public string Password { get; set; }
        public string HomeStreet { get; set; }
        public string HomeCity { get; set; }
        public string HomeCountry { get; set; }
        public string HomeZipCode { get; set; }
        public string OfficeStreet { get; set; }
        public string OfficeCity { get; set; }
        public string OfficeCountry { get; set; }
        public string OfficeZipCode { get; set; }
        public bool IsActive { get; set; }
        public bool MaritalStatus { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? Anniversary { get; set; }
        public DateTime? JoinedDate { get; set; }
        public bool IsReseller { get; set; }
        public List<ApplicationProductModels> ListProduct { get; set; }
        public byte[] PictureByte { get; set; }
        [DataType(DataType.Upload)]
        [FileTypes("jpeg,jpg,png,bmp")]
        public HttpPostedFileBase PictureUpload { get; set; }
        public List<CountryModel> ListCountry { get; set; }
        public string MerchantName { get; set; }
        public Customermodel()
        {
            ListProduct = new List<ApplicationProductModels>();
            ListCountry = new List<CountryModel>();
        }
    }

    public class PurchaseReceiptModels
    {
        public string MerchantID { get; set; }
        public string MerchantName { get; set; }

        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }

        public List<OrderModels> ListReceipt { get; set; }
        public PurchaseReceiptModels()
        {
            ListReceipt = new List<OrderModels>();
        }
    }

    public class OrderModels
    {
        public string ID { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime PaidTime { get; set; }
        public int NumberOfItem { get; set; }
        public double Total { get; set; }
        public List<string> PaidByMethod { get; set; }
        public string sPaidByMethod { get; set; }
        public int OrderStatus { get; set; }
        public string sOrderStatus { get; set; }
        public string CurrencySymbol { get; set; }
        //for Reseller
        public string CustomerEmail { get; set; }
        public string MerchantName { get; set; }
        public string CustomerName { get; set; }
    }

    public class ApplicationProductModels
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public List<ProductPriceModels> ListPrice { get; set; }
    }
}
