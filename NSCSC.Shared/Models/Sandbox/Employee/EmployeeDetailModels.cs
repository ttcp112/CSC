using System;
using System.Collections.Generic;
using NSCSC.Shared.Models.Base;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using NSCSC.Shared.Factory.Settings.Permissions;
using System.Linq;
using NSCSC.Shared.Models.CRM.Customers;
using NSCSC.Shared.Utilities;
using System.Configuration;

namespace NSCSC.Shared.Models.Sandbox.Employee
{
    public class EmployeeDetailModels : BaseModels
    {
        public string ID { get; set; }

        [Display(Name = "Role")]

        [Required]
        public string RoleID { get; set; }

        public string RoleName { get; set; }

        [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+", ErrorMessage = "Please enter correct email.")]

        [Required]

        public string Email { get; set; }

        [Required]

        public string Name { get; set; }

        [RegularExpression(@"[0-9()+-]+", ErrorMessage = "Please only enter number, (, ), -, +")]

        [Required]

        public string Phone { get; set; }

        public string IC { get; set; }

        [Display(Name = "Pin Code")]

        [RegularExpression(@"[0-9]+", ErrorMessage = "Please only enter number.")]

        [Required]

        public string PinCode { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        [Required]

        public string Country { get; set; }

        [Required]

        public string ZipCode { get; set; }

        [Required]

        public bool IsActive { get; set; }

        public bool IsMarital { get; set; }

        public List<SelectListItem> ListMarital { get; set; }
        public List<CountryModel> ListCountry { get; set; }

        public bool Gender { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:" + Commons.FormatDate + "}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime Birthday { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:" + Commons.FormatDate + "}")]

        public DateTime HiredDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:" + Commons.FormatDate + "}")]

        public DateTime ExpiredDate { get; set; }

        public List<SelectListItem> ListRole { get; set; }

        [DataType(DataType.Password)]

        [Display(Name = "Old Password")]

        public string OldPassword { get; set; }

        [DataType(DataType.Password)]

        [Display(Name = "New Password")]

        public string NewPassword { get; set; }

        [DataType(DataType.Password)]

        [Display(Name = "Comfirm Password")]

        public string ConfirmPassword { get; set; }
        public string Error { get; set; }

        public EmployeeDetailModels()
        {
            IsActive = true;
            Birthday = DateTime.Now;
            HiredDate = DateTime.Now;
            ExpiredDate = DateTime.Now;
            Gender = true;

            ListMarital = new List<SelectListItem>()
            {
                new SelectListItem() { Text = Commons.MaritalStatusSingle, Value = "False"},
                new SelectListItem() { Text = Commons.MaritalStatusMarried, Value = "True"}
            };
            ListCountry = new List<CountryModel>();
        }

        public void GetListRole(int level)
        {
            ListRole = new List<SelectListItem>();
            PermissionsFactory permissionsFactory = new PermissionsFactory();
            List<SelectListItem> slcRole = new List<SelectListItem>();
            var listRole = permissionsFactory.GetListData(level).Where(x=>x.IsActive).OrderBy(o=>o.Name).ToList();
            foreach (var item in listRole)
            {
                ListRole.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.ID
                });
            }
        }
    }
}
