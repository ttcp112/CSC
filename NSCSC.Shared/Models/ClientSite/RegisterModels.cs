using NSCSC.Shared.Factory;
using NSCSC.Shared.Models.CRM.Customers;
using NSCSC.Shared.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NSCSC.Shared.Models.ClientSite
{
    public class RegisterModels
    {        
        public CustomerDetailDTO CustomerDetail { get; set;}
        public MerchantDTO MerchantDetail { get; set; }        
        public bool IsCheckPolicies { get; set; }
        public string VerificationCode { get; set; }
        public List<CountryModel> ListCounTry { get; set; }
        public RegisterModels()
        {
            BaseFactory _BaseFactory = new BaseFactory();
            CustomerDetail = new CustomerDetailDTO();
            MerchantDetail = new MerchantDTO();
            ListCounTry = new List<CountryModel>();
        }
        public class StoreDTO
        {
            public string ID { get; set; }
            public string ImageURL { get; set; }
            public string Name { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedDate { get; set; }
            public List<ApplyProductOnStore> ListApplyProduct { get; set; }
        }
        public class ApplyProductOnStore
        {
            public string OrderID { get; set; }
            public string OrderNo { get; set; }
            public string ProductID { get; set; }
            public string ProductName { get; set; }
            public int Period { get; set; }
            public int PeriodType { get; set; }
        }           
    }

    public class ResendEmailModels
    {
        public string Email { get; set; }
    }
    public class VerificationModels
    {
        public string VerificationCode { get; set; }
        public string ReSendEmail { get; set; }
        public bool Result { get; set; }
        public VerificationModels()
        {
            Result = true;
        }
    }
    //Customer
    public class CustomerDetailDTO
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
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
        public string ConfirmPassword { get; set; }        
[DataType(DataType.Upload)]
        //[FileSize(10072000)]
        //[FileSize(300000)]
        [FileTypes("jpeg,jpg,png")]
        public HttpPostedFileBase PictureUpload { get; set; }
        public byte[] PictureByte { get; set; }
        public CustomerDetailDTO() {
            Gender = true;
        }
    }
    //Merchant
    public class MerchantDTO
    {
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string ImageURL { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string GSTRegNo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public List<StoreDTO> ListStore { get; set; }
        public string ConfirmPassword { get; set; }
        public string Address { get; set; }
        [DataType(DataType.Upload)]
        //[FileSize(10072000)]
        //[FileSize(300000)]
        [FileTypes("jpeg,jpg,png")]
        public HttpPostedFileBase PictureUpload { get; set; }
        public byte[] PictureByte { get; set; }
    }

    public class StoreDTO
    {
        public string ID { get; set; }
        public string ImageURL { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<ApplyProductOnStore> ListApplyProduct { get; set; }
    }

    public class ApplyProductOnStore
    {
        public string OrderID { get; set; }
        public string OrderNo { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public int Period { get; set; }
        public int PeriodType { get; set; }
    }
}
