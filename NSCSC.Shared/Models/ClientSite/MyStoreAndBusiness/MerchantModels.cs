using NSCSC.Shared.Models.CRM.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace NSCSC.Shared.Models.ClientSite.MyStoreAndBusiness
{
    public class MerchantModels
    {
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CusMerchantID { get; set; }
        public string ImageURL { get; set; }
        [Required]
        public string Name { get; set; }
        public string CloudWebHQ { get; set; }
        public string Phone { get; set; }
        public string MaskPhone { get; set; }
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        public string MaskEmail { get; set; }
        public string GSTRegNo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public List<StoreModels> ListStore { get; set; }
        [DataType(DataType.Upload)]
        public HttpPostedFileBase PictureUpload { get; set; }
        public byte[] PictureByte { get; set; }
        public List<CountryModel> ListCounTry { get; set; }
        public int NumOfStore { get; set; }
        public MerchantModels()
        {
            ListStore = new List<StoreModels>();
            ListCounTry = new List<CountryModel>();
        }
    }
}
