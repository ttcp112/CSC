using NSCSC.Shared.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.CRM.Customers
{
    public class StoreDetailModels
    {
        public string ID { get; set; }
        public string MerchantID { get; set; }
        public int StoreType { get; set; }
        public List<SelectListItem> StoreTypes { get; set; }
        public string ImageURL { get; set; }
        [Required]
        public string Name { get; set; }
        
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        [Required]
        public string Email { get; set; }
     
        [RegularExpression(@"[0-9()+-]+", ErrorMessage = "Please only enter number, (, ), -, +")]
        [Required]
        public string Phone { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public string ZipCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpriedDate { get; set; }
        public string Description { get; set; }
        public string Industry { get; set; }
        public List<SelectListItem> ListIndustry { get; set; }
        public string GSTRegNo { get; set; }
        public DateTime? dTimezone { get; set; }
        public List<ApplyProductOnStoreModels> ListApplyProduct { get; set; }

        [FileTypes("jpeg,jpg,png")]
        public HttpPostedFileBase PictureUpload { get; set; }
        public byte[] PictureByte { get; set; }
        public string AssetID { get; set; }
        public bool IsNew { get; set; }
        [Required]
        public String TimeZone { get; set; }

        public List<SelectListItem> ListItemProduct;
        public List<SelectListItem> ListTypePackage;
        public string ProductID { get; set; }
        public ProductApplyStoreModels ProductApplyStore { get; set; }
        public string CustomerID { get; set; }
        public List<StoreModels> ListStore { get; set; }
        public string tempAssetID { get; set; }
        public string CreateUser { get; set; }
        public string ParenIdPackage { get; set; }

        public string tempName { get; set;  }
        public string tempEmail { get; set; }
        public string tempCountry { get; set; }
        public string tempPhone { get; set; }
        public string tempTimeZone { get; set; }
        public string tempStreet { get; set; }
        public string tempCity { get; set; }
        public string tempGSTRegNo { get; set; }
        public string tempZipCode { get; set; }
        public int Type { get; set; }
        public int ProductType { get; set; }

        //This store apply for product
        public  List<ProductApply> ListProductApply { get; set; }
        public StoreDetailModels()
        {
            ListApplyProduct = new List<ApplyProductOnStoreModels>();
            StoreTypes = new List<SelectListItem>();
            ListItemProduct = new List<SelectListItem>();
            ProductApplyStore = new ProductApplyStoreModels();
            ListStore = new List<StoreModels>();
            ListTypePackage = new List<SelectListItem>();
            ListProductApply = new List<ProductApply>();
            ListIndustry = new List<SelectListItem>()
            {
                new SelectListItem() {  Text =  Commons.StoreTypeFnB, Value = Commons.EStoreType.FnB.ToString("d") },
                new SelectListItem() {  Text =  Commons.StoreTypeBeauty, Value = Commons.EStoreType.Beauty.ToString("d") },
            };
        }
        public class ProductApply
        {
            public int OffSet { get; set; }
            public string AssetID { get; set; }
            public string ProductName { get; set; }
            public DateTime? ExpiredTime { get; set; }
            public bool IsApply { get; set; }
            public DateTime? ActiveTime { get; set; }
            public  int StoreType { get; set; }
        }
    }
}
