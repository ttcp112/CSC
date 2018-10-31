using NSCSC.Shared.Models.CRM.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
{
    public class StoreDetailModels
    {
        public int Index { get; set; }

        public string ID { get; set; }
        public string MerchantID { get; set; }
        public int StoreType { get; set; }
        public string ImageURL { get; set; }
        public string ProductID { get; set; }

        public string TempName { get; set; }
        public string TempEmail { get; set; }
        public string TempPhone { get; set; }
        public string TempGSTRegNo { get; set; }
        public string TempCity { get; set; }
        public string TempZipCode { get; set;}
        public string TempStreet { get; set; }
        public string TempCounTry { get; set; }
        public string TempTimeZone { get; set; }



        //[Required]
        public string Name { get; set; }
       // [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        //[Required]
        //[RegularExpression("[-+()]\\d+", ErrorMessage = "Phone must be numeric")]
        public string Phone { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpriedDate { get; set; }
        public string Description { get; set; }
        //[Required]
        //[RegularExpression("\\d+", ErrorMessage = "Time Zone must be numeric")]
        public string TimeZone { get; set; }
        public string GSTRegNo { get; set; }
        //[Required]
        public string Industry { get; set; }
        public List<ApplyProductOnStoreModels> ListApplyProduct { get; set; }

        public string ApplyProductCount { get; set; }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase PictureUpload { get; set; }
        public byte[] PictureByte { get; set; }

        public bool IsNew { get; set; }
        public bool IsExistStore { get; set; }

        public List<SelectListItem> ListIndustry { get; set; }

        public string sAssetID { get; set; }
        public string sID { get; set; }
        public string AssetID { get; set; }

        public string ProductAppliedId { get; set; }
        public List<SelectListItem> ListApplyProductClient { get; set; }
        public int ProductType { get; set; }

        public bool IsDelete { get; set; }
        public string StoreIDApply { get; set; }
        public string StoreNameExist { get; set; }
        public List<SelectListItem> ListTimeZone { get; set; }
        public List<SelectListItem> ListTypePackage { get; set; }
        public int Type { get; set; }
        public List<ProductApply> ListProductApply { get; set; }
        public List<string> ListAssetID { get; set; }
        //----------
        public string _action { get; set; }//AddNew|Update
        public List<CountryModel> Countries { get; set; }
        public List<string> TimeZones { get; set; }
        public string CusID { get; set; }
        public StoreDetailModels()
        {
            ListApplyProduct = new List<ApplyProductOnStoreModels>();
            ListApplyProductClient = new List<SelectListItem>();
            ListTimeZone = new List<SelectListItem>();
            ListTypePackage = new List<SelectListItem>();
            ListProductApply = new List<ProductApply>();
            Countries = new List<CountryModel>();
            TimeZones = new List<string>();
            ListIndustry = new List<SelectListItem>()
            {
                new SelectListItem() {  Text =  Commons.StoreTypeFnB, Value = Commons.EStoreType.FnB.ToString("d") },
                new SelectListItem() {  Text =  Commons.StoreTypeBeauty, Value = Commons.EStoreType.Beauty.ToString("d") },
            };
            ListAssetID = new List<string>();
            //===================
            _action = "add_new";
            IsNew = true;
        }
    }
    public class ProductApply
    {
        public int OffSet { get; set; }
        public string AssetID { get; set; }
        public string ProductName { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public string ExpiredTimeDisplay { get; set; }
        public bool IsApply { get; set; }
        public DateTime? ActiveTime { get; set; }
        public string sActiveTime { get; set; }
        public string sExpiredTime { get; set; }
    }
}
