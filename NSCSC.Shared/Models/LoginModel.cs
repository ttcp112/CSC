using NSCSC.Shared.Models.SandBox.Inventory.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models
{
    public class LoginRequestModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

    }
    public class LoginResponseModel
    {
        //public string MerchantID { get; set; }
        //public string MerchantName { get; set; }

        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeeImageURL { get; set; }
        public string EMployeePincode { get; set; }

        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public int RoleLevel { get; set; }

        public string PublicImages { get; set; }
        public string FTPWebImage { get; set; }
        public string FTPUser { get; set; }
        public string FTPPassword { get; set; }

        public List<PermissionDTO> ListPermission { get; set; }
        //public List<FunctionDTO> ListFunction { get; set; }
        //public List<StoreWebDTO> ListStore { get; set; }
        public string CurrencySymbol { get; set; }

        //public bool IsIntergrate { get; set; }
        public string OrderID { get; set; }

        public LoginResponseModel()
        {
            //ListStore = new List<StoreWebDTO>();
            ListPermission = new List<PermissionDTO>();
        }
    }
    //public class StoreWebDTO
    //{
    //    public string ID { get; set; }
    //    public string Name { get; set; }
    //    public int IndustryCode { get; set; }
    //}
    //public class FunctionDTO
    //{
    //    public string ID { get; set; }
    //    public string Name { get; set; }
    //    public bool IsAction { get; set; }
    //    public bool IsView { get; set; }
    //    public bool IsActive { get; set; }
    //    public int Code { get; set; }
    //    public List<FunctionDTO> ListChild { get; set; }
    //}

    public class PermissionDTO
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public bool IsView { get; set; }
        public bool IsAction { get; set; }
        public bool Checked { get; set; }
        public List<PermissionDTO> ListChild { get; set; }

        public string RoleID { get; set; }
        public string ModuleID { get; set; }
        public string ModuleParentID { get; set; }
    }

    //============Client Model====================

    public class AccountLoginRequest
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
    public class ResponseAccountLogin
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ImageURL { get; set; }
        public bool IsVerify { get; set; }
        public string OrderID { get; set; }
        public int NumOfItems { get; set; }
        public bool IsReseller { get; set; }
        public string CurrencySymbol { get; set; }
        public List<ProductDetailModels> ListProduct { get; set; }
    }
    public class AccountForgotPasswordRequest
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
