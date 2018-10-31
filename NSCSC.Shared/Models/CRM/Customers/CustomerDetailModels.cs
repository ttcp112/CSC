using NSCSC.Shared.Models.Base;
using NSCSC.Shared.Models.Sandbox.Inventory.Product;
using NSCSC.Shared.Models.SandBox.Inventory.Product;
using NSCSC.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.CRM.Customers
{
    public class CustomerDetailModels : BaseModels
    {
        public string ID { get; set; }
        [Required]
        public string Name { get; set; }
       
        [Required]
        [RegularExpression(@"[0-9()+-]+", ErrorMessage = "Please only enter number, (, ), -, +")]
        public string Phone { get; set; }
        public string PhoneDisplay { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+", ErrorMessage = "Please enter correct email.")]
        public string Email { get; set; }
        public string EmailDisplay { get; set; }
        public string NRIC { get; set; }
        public string NRICDisplay { get; set; }
        public bool IsActive { get; set; }
        public bool MaritalStatus { get; set; }
        public bool Gender { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Birthday { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Anniversary { get; set; }

        public string HomeStreet { get; set; }
        public string HomeCity { get; set; }
        [Display(Name = "Home Country")]
        [Required]
        public string HomeCountry { get; set; }
        [Display(Name = "Home ZIP Code")]
        [Required]
        public string HomeZipCode { get; set; }

        public string OfficeStreet { get; set; }
        public string OfficeCity { get; set; }
        [Display(Name = "Office Country")]
        [Required]
        public string OfficeCountry { get; set; }
        [Display(Name = "Office ZIP Code")]
        [Required]
        public string OfficeZipCode { get; set; }
        public List<OrderModels> ListReceipt { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime JoinedDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ExpiredDate { get; set; }

        /*For Client*/
        public List<SelectListItem> ListMaritalStatus { get; set; }
        public List<CountryModel> ListCounTry { get; set; }
 
        public int ClientMaritalStatus { get; set; }
        public string MerchantName { get; set; }

        public bool IsReseller { get; set; }
        public List<ApplicationProductModel> ListProduct { get; set; }

        public int PeriodType { get; set; }
        public List<SelectListItem> ListPeriodType { get; set; }

        public List<ProductModels> ListAppProduct { get; set; }        

        public CustomerDetailModels()
        {
            IsActive = true;
            Gender = true;

            Birthday = DateTime.Now;
            Anniversary = DateTime.Now;
            JoinedDate = DateTime.Now;
            ExpiredDate = DateTime.Now;

            ListMaritalStatus = new List<SelectListItem>()
            {
                new SelectListItem() {  Text =  "Single", Value = "0" },
                new SelectListItem() {  Text =  "Married", Value = "1" },
            };
            ListCounTry = new List<CountryModel>();
            ListReceipt = new List<OrderModels>();
            ListProduct = new List<ApplicationProductModel>();
            ListPeriodType = new List<SelectListItem>()
            {
                //new SelectListItem() { Value = Commons.EPeriodType.None.ToString("d"), Text = Commons.PeriodTypeNone },
                new SelectListItem() { Value = Commons.EPeriodType.Day.ToString("d"), Text = Commons.PeriodTypeDay },
                new SelectListItem() { Value = Commons.EPeriodType.Month.ToString("d"), Text = Commons.PeriodTypeMonth },
                new SelectListItem() { Value = Commons.EPeriodType.Year.ToString("d") ,Text = Commons.PeriodTypeYear },
                new SelectListItem() { Value = Commons.EPeriodType.OneTime.ToString("d"),Text = Commons.PeriodTypeOneTime }
            };
            ListAppProduct = new List<ProductModels>();
        }
    }

    public class OrderModels
    {
        public string ID { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public int NumberOfItem { get; set; }
        public double Total { get; set; }
        public List<string> PaidByMethod { get; set; }
        public string PaidByMethodText { get; set; }
        public int OrderStatus { get; set; }
        //
        public string currency { get; set; }
        public DateTime PaidTime { get; set; }
        public OrderModels()
        {
            PaidByMethod = new List<string>();
        }
    }

    public class ApplicationProductModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public byte Status { get; set; }
        public int OffSet { get; set; }
        public List<ProductPriceModels> ListPrice { get; set; }
        public ApplicationProductModel()
        {
            ListPrice = new List<ProductPriceModels>();
        }
    }

    public class ProductOnGroupModels
    {
        public int CurrentOffset { get; set; }
        public List<ProductModels> ListProductOnGroup { get; set; }
        public ProductOnGroupModels()
        {
            ListProductOnGroup = new List<ProductModels>();
        }
    }
}
